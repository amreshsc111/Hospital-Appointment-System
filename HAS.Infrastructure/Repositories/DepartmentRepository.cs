using HAS.Application.Common.Interfaces;
using HAS.Domain.Entities;
using HAS.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace HAS.Infrastructure.Repositories;

public class DepartmentRepository(ApplicationDbContext dbContext) : IDepartmentRepository
{
    private readonly ApplicationDbContext _dbContext = dbContext;

    public Task<Department?> GetByIdAsync(Guid id, CancellationToken cancellationToken) =>
        _dbContext.Departments
            .Include(d => d.Doctors)
            .FirstOrDefaultAsync(d => d.Id == id && !d.IsDeleted, cancellationToken);

    public Task<List<Department>> GetAllAsync(CancellationToken cancellationToken) =>
        _dbContext.Departments
            .Include(d => d.Doctors)
            .Where(d => !d.IsDeleted)
            .ToListAsync(cancellationToken);

    public Task AddAsync(Department department, CancellationToken cancellationToken)
    {
        return _dbContext.Departments.AddAsync(department, cancellationToken).AsTask();
    }

    public Task UpdateAsync(Department department, CancellationToken cancellationToken)
    {
        _dbContext.Departments.Update(department);
        return Task.CompletedTask;
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        var department = await _dbContext.Departments.FindAsync([id], cancellationToken);
        if (department != null)
        {
            department.IsDeleted = true;
            department.DeletedAt = DateTime.UtcNow;
        }
    }

    public Task<bool> ExistsByNameAsync(string name, CancellationToken cancellationToken) =>
        _dbContext.Departments.AnyAsync(d => d.Name == name && !d.IsDeleted, cancellationToken);

    public Task SaveChangesAsync(CancellationToken cancellationToken) =>
        _dbContext.SaveChangesAsync(cancellationToken);
}
