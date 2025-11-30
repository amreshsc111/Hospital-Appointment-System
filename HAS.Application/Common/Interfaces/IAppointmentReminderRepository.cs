using HAS.Domain.Entities;

namespace HAS.Application.Common.Interfaces;

public interface IAppointmentReminderRepository
{
    Task<List<AppointmentReminder>> GetPendingRemindersAsync(DateTime upToTime, CancellationToken cancellationToken);
    Task<AppointmentReminder?> GetByAppointmentIdAsync(Guid appointmentId, CancellationToken cancellationToken);
    Task AddAsync(AppointmentReminder reminder, CancellationToken cancellationToken);
    Task UpdateAsync(AppointmentReminder reminder, CancellationToken cancellationToken);
    Task SaveChangesAsync(CancellationToken cancellationToken);
}
