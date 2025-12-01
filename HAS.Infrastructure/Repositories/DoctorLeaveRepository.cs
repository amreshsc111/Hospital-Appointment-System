using HAS.Application.Common.Interfaces;
using HAS.Domain.Entities;
using HAS.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace HAS.Infrastructure.Repositories;

public class DoctorLeaveRepository(ApplicationDbContext dbContext) : IDoctorLeaveRepository
{
    private readonly ApplicationDbContext _dbContext = dbContext;

    public Task<List<DoctorLeave>> GetByDoctorIdAsync(Guid doctorId, CancellationToken cancellationToken) =>
        _dbContext.Set<DoctorLeave>()
            .Where(l => l.DoctorId == doctorId && !l.IsDeleted)
            .OrderByDescending(l => l.StartDate)
            .ToListAsync(cancellationToken);

    public Task<List<DoctorLeave>> GetActiveLeavesAsync(Guid doctorId, DateTime date, CancellationToken cancellationToken) =>
        _dbContext.Set<DoctorLeave>()
            .Where(l => l.DoctorId == doctorId &&
                       l.IsApproved &&
                       !l.IsDeleted &&
                       l.StartDate <= date &&
                       l.EndDate >= date)
            .ToListAsync(cancellationToken);

    public Task<DoctorLeave?> GetByIdAsync(Guid id, CancellationToken cancellationToken) =>
        _dbContext.Set<DoctorLeave>()
            .FirstOrDefaultAsync(l => l.Id == id && !l.IsDeleted, cancellationToken);

    public Task AddAsync(DoctorLeave leave, CancellationToken cancellationToken)
    {
        return _dbContext.Set<DoctorLeave>().AddAsync(leave, cancellationToken).AsTask();
    }

    public Task UpdateAsync(DoctorLeave leave, CancellationToken cancellationToken)
    {
        _dbContext.Set<DoctorLeave>().Update(leave);
        return Task.CompletedTask;
    }
}
