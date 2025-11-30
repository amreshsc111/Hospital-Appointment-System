using HAS.Application.Common.Interfaces;
using HAS.Domain.Entities;
using HAS.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace HAS.Infrastructure.Repositories;

public class PatientRepository(ApplicationDbContext dbContext) : IPatientRepository
{
    private readonly ApplicationDbContext _dbContext = dbContext;

    public Task<Patient?> GetByIdAsync(Guid id, CancellationToken cancellationToken) =>
        _dbContext.Patients
            .FirstOrDefaultAsync(p => p.Id == id && !p.IsDeleted, cancellationToken);

    public Task<List<Patient>> GetAllAsync(CancellationToken cancellationToken) =>
        _dbContext.Patients
            .Where(p => !p.IsDeleted)
            .ToListAsync(cancellationToken);

    public Task<Patient?> GetByEmailAsync(string email, CancellationToken cancellationToken) =>
        _dbContext.Patients
            .FirstOrDefaultAsync(p => p.Email.Value == email && !p.IsDeleted, cancellationToken);

    public Task<Patient?> GetByPhoneAsync(string phone, CancellationToken cancellationToken) =>
        _dbContext.Patients
            .FirstOrDefaultAsync(p => p.PhoneNumber.Value == phone && !p.IsDeleted, cancellationToken);

    public Task AddAsync(Patient patient, CancellationToken cancellationToken)
    {
        return _dbContext.Patients.AddAsync(patient, cancellationToken).AsTask();
    }

    public Task UpdateAsync(Patient patient, CancellationToken cancellationToken)
    {
        _dbContext.Patients.Update(patient);
        return Task.CompletedTask;
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        var patient = await _dbContext.Patients.FindAsync([id], cancellationToken);
        if (patient != null)
        {
            patient.IsDeleted = true;
            patient.DeletedAt = DateTime.UtcNow;
        }
    }

    public Task<bool> ExistsByEmailAsync(string email, CancellationToken cancellationToken) =>
        _dbContext.Patients.AnyAsync(p => p.Email.Value == email && !p.IsDeleted, cancellationToken);

    public Task SaveChangesAsync(CancellationToken cancellationToken) =>
        _dbContext.SaveChangesAsync(cancellationToken);
}
