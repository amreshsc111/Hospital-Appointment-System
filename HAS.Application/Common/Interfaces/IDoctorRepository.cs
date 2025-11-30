using HAS.Domain.Entities;

namespace HAS.Application.Common.Interfaces;

public interface IDoctorRepository
{
    Task<Domain.Entities.Doctor?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<Domain.Entities.Doctor?> GetByIdWithDepartmentAsync(Guid id, CancellationToken cancellationToken);
    Task<List<Domain.Entities.Doctor>> GetAllAsync(CancellationToken cancellationToken);
    Task<List<Domain.Entities.Doctor>> GetByDepartmentIdAsync(Guid departmentId, CancellationToken cancellationToken);
    Task<List<Domain.Entities.Doctor>> GetAvailableDoctorsAsync(CancellationToken cancellationToken);
    Task AddAsync(Domain.Entities.Doctor doctor, CancellationToken cancellationToken);
    Task UpdateAsync(Domain.Entities.Doctor doctor, CancellationToken cancellationToken);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken);
    Task<bool> ExistsByEmailAsync(string email, CancellationToken cancellationToken);
    Task SaveChangesAsync(CancellationToken cancellationToken);
}
