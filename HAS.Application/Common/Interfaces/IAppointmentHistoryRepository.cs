using HAS.Domain.Entities;

namespace HAS.Application.Common.Interfaces;

public interface IAppointmentHistoryRepository
{
    Task<List<AppointmentHistory>> GetByAppointmentIdAsync(Guid appointmentId, CancellationToken cancellationToken);
    Task AddAsync(AppointmentHistory history, CancellationToken cancellationToken);
    Task SaveChangesAsync(CancellationToken cancellationToken);
}
