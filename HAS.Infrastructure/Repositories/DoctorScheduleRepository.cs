using HAS.Application.Common.Interfaces;
using HAS.Domain.Entities;
using HAS.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace HAS.Infrastructure.Repositories;

public class DoctorScheduleRepository(ApplicationDbContext dbContext) : IDoctorScheduleRepository
{
    private readonly ApplicationDbContext _dbContext = dbContext;

    public Task<List<DoctorSchedule>> GetByDoctorIdAsync(Guid doctorId, CancellationToken cancellationToken) =>
        _dbContext.Set<DoctorSchedule>()
            .Where(s => s.DoctorId == doctorId && !s.IsDeleted)
            .OrderBy(s => s.DayOfWeek)
            .ToListAsync(cancellationToken);

    public Task<DoctorSchedule?> GetByIdAsync(Guid id, CancellationToken cancellationToken) =>
        _dbContext.Set<DoctorSchedule>()
            .FirstOrDefaultAsync(s => s.Id == id && !s.IsDeleted, cancellationToken);

    public Task AddAsync(DoctorSchedule schedule, CancellationToken cancellationToken)
    {
        return _dbContext.Set<DoctorSchedule>().AddAsync(schedule, cancellationToken).AsTask();
    }

    public Task UpdateAsync(DoctorSchedule schedule, CancellationToken cancellationToken)
    {
        _dbContext.Set<DoctorSchedule>().Update(schedule);
        return Task.CompletedTask;
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        var schedule = await _dbContext.Set<DoctorSchedule>().FindAsync([id], cancellationToken);
        if (schedule != null)
        {
            schedule.IsDeleted = true;
            schedule.DeletedAt = DateTime.UtcNow;
        }
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken) =>
        _dbContext.SaveChangesAsync(cancellationToken);
}
