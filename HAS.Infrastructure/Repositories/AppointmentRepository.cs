using HAS.Application.Common.Interfaces;
using HAS.Domain.Entities;
using HAS.Domain.Enums;
using HAS.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace HAS.Infrastructure.Repositories;

public class AppointmentRepository(ApplicationDbContext dbContext) : IAppointmentRepository
{
    private readonly ApplicationDbContext _dbContext = dbContext;

    public Task<Appointment?> GetByIdAsync(Guid id, CancellationToken cancellationToken) =>
        _dbContext.Appointments
            .FirstOrDefaultAsync(a => a.Id == id && !a.IsDeleted, cancellationToken);

    public Task<Appointment?> GetByIdWithDetailsAsync(Guid id, CancellationToken cancellationToken) =>
        _dbContext.Appointments
            .Include(a => a.Doctor)
                .ThenInclude(d => d.Department)
            .Include(a => a.Patient)
            .FirstOrDefaultAsync(a => a.Id == id && !a.IsDeleted, cancellationToken);

    public Task<List<Appointment>> GetAllAsync(CancellationToken cancellationToken) =>
        _dbContext.Appointments
            .Include(a => a.Doctor)
            .Include(a => a.Patient)
            .Where(a => !a.IsDeleted)
            .ToListAsync(cancellationToken);

    public Task<List<Appointment>> GetByDoctorIdAsync(Guid doctorId, CancellationToken cancellationToken) =>
        _dbContext.Appointments
            .Include(a => a.Doctor)
            .Include(a => a.Patient)
            .Where(a => a.DoctorId == doctorId && !a.IsDeleted)
            .ToListAsync(cancellationToken);

    public Task<List<Appointment>> GetByPatientIdAsync(Guid patientId, CancellationToken cancellationToken) =>
        _dbContext.Appointments
            .Include(a => a.Doctor)
            .Include(a => a.Patient)
            .Where(a => a.PatientId == patientId && !a.IsDeleted)
            .ToListAsync(cancellationToken);

    public Task<List<Appointment>> GetByDoctorAndDateAsync(Guid doctorId, DateTime date, CancellationToken cancellationToken)
    {
        var startOfDay = date.Date;
        var endOfDay = date.Date.AddDays(1);

        return _dbContext.Appointments
            .Include(a => a.Doctor)
            .Include(a => a.Patient)
            .Where(a => a.DoctorId == doctorId && 
                       a.StartAt >= startOfDay && 
                       a.StartAt < endOfDay && 
                       !a.IsDeleted)
            .ToListAsync(cancellationToken);
    }

    public Task<List<Appointment>> GetByDateRangeAsync(DateTime startDate, DateTime endDate, CancellationToken cancellationToken) =>
        _dbContext.Appointments
            .Include(a => a.Doctor)
            .Include(a => a.Patient)
            .Where(a => a.StartAt >= startDate && a.StartAt <= endDate && !a.IsDeleted)
            .ToListAsync(cancellationToken);

    public Task<List<Appointment>> GetByStatusAsync(AppointmentStatus status, CancellationToken cancellationToken) =>
        _dbContext.Appointments
            .Include(a => a.Doctor)
            .Include(a => a.Patient)
            .Where(a => a.Status == status && !a.IsDeleted)
            .ToListAsync(cancellationToken);

    public Task<bool> HasConflictAsync(
        Guid doctorId, 
        DateTime startAt, 
        DateTime endAt, 
        Guid? excludeAppointmentId, 
        CancellationToken cancellationToken)
    {
        var query = _dbContext.Appointments
            .Where(a => a.DoctorId == doctorId && 
                       !a.IsDeleted &&
                       (a.Status == AppointmentStatus.Pending || a.Status == AppointmentStatus.Confirmed) &&
                       ((startAt >= a.StartAt && startAt < a.EndAt) ||
                        (endAt > a.StartAt && endAt <= a.EndAt) ||
                        (startAt <= a.StartAt && endAt >= a.EndAt)));

        if (excludeAppointmentId.HasValue)
        {
            query = query.Where(a => a.Id != excludeAppointmentId.Value);
        }

        return query.AnyAsync(cancellationToken);
    }

    public Task AddAsync(Appointment appointment, CancellationToken cancellationToken)
    {
        return _dbContext.Appointments.AddAsync(appointment, cancellationToken).AsTask();
    }

    public Task UpdateAsync(Appointment appointment, CancellationToken cancellationToken)
    {
        _dbContext.Appointments.Update(appointment);
        return Task.CompletedTask;
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        var appointment = await _dbContext.Appointments.FindAsync([id], cancellationToken);
        if (appointment != null)
        {
            appointment.IsDeleted = true;
            appointment.DeletedAt = DateTime.UtcNow;
        }
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken) =>
        _dbContext.SaveChangesAsync(cancellationToken);
}
