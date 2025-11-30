using HAS.Application.Common.Interfaces;
using HAS.Domain.Entities;
using HAS.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace HAS.Infrastructure.Repositories;

public class CancellationPolicyRepository(ApplicationDbContext dbContext) : ICancellationPolicyRepository
{
    private readonly ApplicationDbContext _dbContext = dbContext;

    public Task<CancellationPolicy?> GetByDoctorIdAsync(Guid doctorId, CancellationToken cancellationToken) =>
        _dbContext.Set<CancellationPolicy>()
            .FirstOrDefaultAsync(p => p.DoctorId == doctorId && !p.IsDeleted, cancellationToken);

    public Task AddAsync(CancellationPolicy policy, CancellationToken cancellationToken)
    {
        return _dbContext.Set<CancellationPolicy>().AddAsync(policy, cancellationToken).AsTask();
    }

    public Task UpdateAsync(CancellationPolicy policy, CancellationToken cancellationToken)
    {
        _dbContext.Set<CancellationPolicy>().Update(policy);
        return Task.CompletedTask;
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken) =>
        _dbContext.SaveChangesAsync(cancellationToken);
}
