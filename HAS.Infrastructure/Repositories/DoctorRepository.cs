using HAS.Application.Common.Interfaces;
using HAS.Domain.Entities;
using HAS.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace HAS.Infrastructure.Repositories;

public class DoctorRepository(ApplicationDbContext dbContext) : IDoctorRepository
{
    private readonly ApplicationDbContext _dbContext = dbContext;

    public Task<Doctor?> GetByIdAsync(Guid id, CancellationToken cancellationToken) =>
        _dbContext.Doctors
            .FirstOrDefaultAsync(d => d.Id == id && !d.IsDeleted, cancellationToken);

    public Task<Doctor?> GetByIdWithDepartmentAsync(Guid id, CancellationToken cancellationToken) =>
        _dbContext.Doctors
            .Include(d => d.Department)
            .FirstOrDefaultAsync(d => d.Id == id && !d.IsDeleted, cancellationToken);

    public Task<List<Doctor>> GetAllAsync(CancellationToken cancellationToken) =>
        _dbContext.Doctors
            .Include(d => d.Department)
            .Where(d => !d.IsDeleted)
            .ToListAsync(cancellationToken);

    public Task<List<Doctor>> GetByDepartmentIdAsync(Guid departmentId, CancellationToken cancellationToken) =>
        _dbContext.Doctors
            .Include(d => d.Department)
            .Where(d => d.DepartmentId == departmentId && !d.IsDeleted)
            .ToListAsync(cancellationToken);

    public Task<List<Doctor>> GetAvailableDoctorsAsync(CancellationToken cancellationToken) =>
        _dbContext.Doctors
            .Include(d => d.Department)
            .Where(d => d.IsAvailable && !d.IsDeleted)
            .ToListAsync(cancellationToken);

    public Task AddAsync(Doctor doctor, CancellationToken cancellationToken)
    {
        return _dbContext.Doctors.AddAsync(doctor, cancellationToken).AsTask();
    }

    public Task UpdateAsync(Doctor doctor, CancellationToken cancellationToken)
    {
        _dbContext.Doctors.Update(doctor);
        return Task.CompletedTask;
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        var doctor = await _dbContext.Doctors.FindAsync([id], cancellationToken);
        if (doctor != null)
        {
            doctor.IsDeleted = true;
            doctor.DeletedAt = DateTime.UtcNow;
        }
    }

    public Task<bool> ExistsByEmailAsync(string email, CancellationToken cancellationToken) =>
        _dbContext.Doctors.AnyAsync(d => d.Email.Value == email && !d.IsDeleted, cancellationToken);

    public Task SaveChangesAsync(CancellationToken cancellationToken) =>
        _dbContext.SaveChangesAsync(cancellationToken);
}
