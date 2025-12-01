using HAS.Application.Common.Interfaces;
using HAS.Domain.Entities;
using HAS.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace HAS.Infrastructure.Repositories;

public class AppointmentHistoryRepository(ApplicationDbContext dbContext) : IAppointmentHistoryRepository
{
    private readonly ApplicationDbContext _dbContext = dbContext;

    public Task<List<AppointmentHistory>> GetByAppointmentIdAsync(Guid appointmentId, CancellationToken cancellationToken) =>
        _dbContext.Set<AppointmentHistory>()
            .Where(h => h.AppointmentId == appointmentId)
            .OrderByDescending(h => h.CreatedAt)
            .ToListAsync(cancellationToken);

    public Task AddAsync(AppointmentHistory history, CancellationToken cancellationToken)
    {
        return _dbContext.Set<AppointmentHistory>().AddAsync(history, cancellationToken).AsTask();
    }
}
