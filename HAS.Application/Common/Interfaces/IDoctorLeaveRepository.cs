using HAS.Domain.Entities;

namespace HAS.Application.Common.Interfaces;

public interface IDoctorLeaveRepository
{
    Task<List<Domain.Entities.DoctorLeave>> GetByDoctorIdAsync(Guid doctorId, CancellationToken cancellationToken);
    Task<List<Domain.Entities.DoctorLeave>> GetActiveLeavesAsync(Guid doctorId, DateTime date, CancellationToken cancellationToken);
    Task<Domain.Entities.DoctorLeave?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task AddAsync(Domain.Entities.DoctorLeave leave, CancellationToken cancellationToken);
    Task UpdateAsync(Domain.Entities.DoctorLeave leave, CancellationToken cancellationToken);
}
