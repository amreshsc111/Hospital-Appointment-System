using HAS.Domain.Entities;

namespace HAS.Application.Common.Interfaces;

public interface ICancellationPolicyRepository
{
    Task<Domain.Entities.CancellationPolicy?> GetByDoctorIdAsync(Guid doctorId, CancellationToken cancellationToken);
    Task AddAsync(Domain.Entities.CancellationPolicy policy, CancellationToken cancellationToken);
    Task UpdateAsync(Domain.Entities.CancellationPolicy policy, CancellationToken cancellationToken);
}
