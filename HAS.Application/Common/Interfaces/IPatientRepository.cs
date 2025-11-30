using HAS.Domain.Entities;

namespace HAS.Application.Common.Interfaces;

public interface IPatientRepository
{
    Task<Domain.Entities.Patient?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<List<Domain.Entities.Patient>> GetAllAsync(CancellationToken cancellationToken);
    Task<Domain.Entities.Patient?> GetByEmailAsync(string email, CancellationToken cancellationToken);
    Task<Domain.Entities.Patient?> GetByPhoneAsync(string phone, CancellationToken cancellationToken);
    Task AddAsync(Domain.Entities.Patient patient, CancellationToken cancellationToken);
    Task UpdateAsync(Domain.Entities.Patient patient, CancellationToken cancellationToken);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken);
    Task<bool> ExistsByEmailAsync(string email, CancellationToken cancellationToken);
    Task SaveChangesAsync(CancellationToken cancellationToken);
}
