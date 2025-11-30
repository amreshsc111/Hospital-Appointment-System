using HAS.Application.Common.Interfaces;
using HAS.Domain.Entities;
using HAS.Domain.Enums;

namespace HAS.Application.Appointment.Commands;

public record ConfirmAppointmentCommand(Guid Id) : IRequest<bool>;

public class ConfirmAppointmentHandler(
    IAppointmentRepository appointments,
    IAppointmentHistoryRepository history,
    IEmailService emailService) 
    : IRequestHandler<ConfirmAppointmentCommand, bool>
{
    private readonly IAppointmentRepository _appointments = appointments;
    private readonly IAppointmentHistoryRepository _history = history;
    private readonly IEmailService _emailService = emailService;

    public async Task<bool> Handle(ConfirmAppointmentCommand request, CancellationToken cancellationToken)
    {
        var appointment = await _appointments.GetByIdWithDetailsAsync(request.Id, cancellationToken);
        if (appointment == null)
            throw new Exception($"Appointment with ID '{request.Id}' not found");

        if (appointment.Status != AppointmentStatus.Pending)
            throw new Exception($"Only pending appointments can be confirmed. Current status: {appointment.Status}");

        var oldStatus = appointment.Status;
        appointment.Status = AppointmentStatus.Confirmed;

        await _appointments.UpdateAsync(appointment, cancellationToken);
        await _appointments.SaveChangesAsync(cancellationToken);

        // Add history record
        var historyRecord = new AppointmentHistory
        {
            Id = Guid.NewGuid(),
            AppointmentId = appointment.Id,
            OldStatus = oldStatus,
            NewStatus = AppointmentStatus.Confirmed,
            Action = "Confirmed",
            ChangedByUserId = Guid.Empty
        };
        await _history.AddAsync(historyRecord, cancellationToken);
        await _history.SaveChangesAsync(cancellationToken);

        // Send confirmation email
        try
        {
            await _emailService.SendAppointmentConfirmationAsync(
                appointment.Patient.Email.Value,
                appointment.Patient.FullName,
                appointment.Doctor.FullName,
                appointment.StartAt,
                appointment.Doctor.Department?.Name ?? "Unknown",
                cancellationToken
            );
        }
        catch
        {
            // Log email failure but don't fail the confirmation
        }

        return true;
    }
}
