using HAS.Domain.Entities;

namespace HAS.Application.Common.Interfaces;

public interface IDoctorScheduleRepository
{
    Task<List<Domain.Entities.DoctorSchedule>> GetByDoctorIdAsync(Guid doctorId, CancellationToken cancellationToken);
    Task<Domain.Entities.DoctorSchedule?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task AddAsync(Domain.Entities.DoctorSchedule schedule, CancellationToken cancellationToken);
    Task UpdateAsync(Domain.Entities.DoctorSchedule schedule, CancellationToken cancellationToken);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken);
}
