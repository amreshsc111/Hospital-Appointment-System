using HAS.Domain.Entities;

namespace HAS.Application.Common.Interfaces;

public interface IDepartmentRepository
{
    Task<Domain.Entities.Department?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<List<Domain.Entities.Department>> GetAllAsync(CancellationToken cancellationToken);
    Task AddAsync(Domain.Entities.Department department, CancellationToken cancellationToken);
    Task UpdateAsync(Domain.Entities.Department department, CancellationToken cancellationToken);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken);
    Task<bool> ExistsByNameAsync(string name, CancellationToken cancellationToken);
}
