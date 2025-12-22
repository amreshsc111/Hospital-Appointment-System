using HAS.Application.Common.Interfaces;
using HAS.Domain.Enums;
using Microsoft.Extensions.Logging;

namespace HAS.Job.Jobs;

public class ReminderBackgroundJob(
    IUnitOfWork unitOfWork,
    IEmailService emailService,
    ILogger<ReminderBackgroundJob> logger)
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IEmailService _emailService = emailService;
    private readonly ILogger<ReminderBackgroundJob> _logger = logger;

    public async Task SendRemindersAsync()
    {
        _logger.LogInformation("Starting reminder background job...");

        try
        {
            // Get pending reminders scheduled for now or in the past
            var pendingReminders = await _unitOfWork.AppointmentReminders.GetPendingRemindersAsync(DateTime.UtcNow, CancellationToken.None);
            
            _logger.LogInformation($"Found {pendingReminders.Count} pending reminders.");

            foreach (var reminder in pendingReminders)
            {
                try
                {
                    await _emailService.SendAppointmentReminderAsync(
                        reminder.Appointment.Patient.Email.Value,
                        reminder.Appointment.Patient.FullName,
                        reminder.Appointment.Doctor.FullName,
                        reminder.Appointment.StartAt,
                        reminder.Appointment.Doctor.Department?.Name ?? "Unknown",
                        CancellationToken.None
                    );

                    reminder.Status = ReminderStatus.Sent;
                    reminder.SentAt = DateTime.UtcNow;
                    
                    await _unitOfWork.AppointmentReminders.UpdateAsync(reminder, CancellationToken.None);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Failed to send reminder for appointment {reminder.AppointmentId}");
                    
                    reminder.RetryCount++;
                    if (reminder.RetryCount >= 3)
                    {
                        reminder.Status = ReminderStatus.Failed;
                        reminder.ErrorMessage = ex.Message;
                    }
                    
                    await _unitOfWork.AppointmentReminders.UpdateAsync(reminder, CancellationToken.None);
                }
            }

            await _unitOfWork.SaveChangesAsync(CancellationToken.None);
            
            _logger.LogInformation("Reminder background job completed.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while executing reminder background job");
            throw;
        }
    }
}
