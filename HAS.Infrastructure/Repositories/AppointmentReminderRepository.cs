using HAS.Application.Common.Interfaces;
using HAS.Domain.Entities;
using HAS.Domain.Enums;
using HAS.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace HAS.Infrastructure.Repositories;

public class AppointmentReminderRepository(ApplicationDbContext dbContext) : IAppointmentReminderRepository
{
    private readonly ApplicationDbContext _dbContext = dbContext;

    public Task<List<AppointmentReminder>> GetPendingRemindersAsync(DateTime upToTime, CancellationToken cancellationToken) =>
        _dbContext.Set<AppointmentReminder>()
            .Include(r => r.Appointment)
                .ThenInclude(a => a.Patient)
            .Include(r => r.Appointment)
                .ThenInclude(a => a.Doctor)
                    .ThenInclude(d => d.Department)
            .Where(r => r.Status == ReminderStatus.Pending &&
                       r.ScheduledFor <= upToTime &&
                       !r.IsDeleted)
            .ToListAsync(cancellationToken);

    public Task<AppointmentReminder?> GetByAppointmentIdAsync(Guid appointmentId, CancellationToken cancellationToken) =>
        _dbContext.Set<AppointmentReminder>()
            .FirstOrDefaultAsync(r => r.AppointmentId == appointmentId && !r.IsDeleted, cancellationToken);

    public Task AddAsync(AppointmentReminder reminder, CancellationToken cancellationToken)
    {
        return _dbContext.Set<AppointmentReminder>().AddAsync(reminder, cancellationToken).AsTask();
    }

    public Task UpdateAsync(AppointmentReminder reminder, CancellationToken cancellationToken)
    {
        _dbContext.Set<AppointmentReminder>().Update(reminder);
        return Task.CompletedTask;
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken) =>
        _dbContext.SaveChangesAsync(cancellationToken);
}
