using HAS.Application.Common.Interfaces;
using HAS.Infrastructure.Persistence.Context;
using HAS.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore.Storage;

namespace HAS.Infrastructure.Persistence;

public class UnitOfWork : IUnitOfWork
{
    private readonly ApplicationDbContext _context;
    private IDbContextTransaction? _transaction;
    
    // Lazy-loaded repositories
    private IUserRepository? _users;
    private IRoleRepository? _roles;
    private IRefreshTokenRepository? _refreshTokens;
    private IDepartmentRepository? _departments;
    private IDoctorRepository? _doctors;
    private IPatientRepository? _patients;
    private IAppointmentRepository? _appointments;
    private IDoctorScheduleRepository? _doctorSchedules;
    private IDoctorLeaveRepository? _doctorLeaves;
    private IAppointmentHistoryRepository? _appointmentHistories;
    private IAppointmentReminderRepository? _appointmentReminders;
    private ICancellationPolicyRepository? _cancellationPolicies;

    public UnitOfWork(ApplicationDbContext context)
    {
        _context = context;
    }

    // Repository Properties (lazy initialization)
    public IUserRepository Users => _users ??= new UserRepository(_context);
    public IRoleRepository Roles => _roles ??= new RoleRepository(_context);
    public IRefreshTokenRepository RefreshTokens => _refreshTokens ??= new RefreshTokenRepository(_context);
    public IDepartmentRepository Departments => _departments ??= new DepartmentRepository(_context);
    public IDoctorRepository Doctors => _doctors ??= new DoctorRepository(_context);
    public IPatientRepository Patients => _patients ??= new PatientRepository(_context);
    public IAppointmentRepository Appointments => _appointments ??= new AppointmentRepository(_context);
    public IDoctorScheduleRepository DoctorSchedules => _doctorSchedules ??= new DoctorScheduleRepository(_context);
    public IDoctorLeaveRepository DoctorLeaves => _doctorLeaves ??= new DoctorLeaveRepository(_context);
    public IAppointmentHistoryRepository AppointmentHistories => _appointmentHistories ??= new AppointmentHistoryRepository(_context);
    public IAppointmentReminderRepository AppointmentReminders => _appointmentReminders ??= new AppointmentReminderRepository(_context);
    public ICancellationPolicyRepository CancellationPolicies => _cancellationPolicies ??= new CancellationPolicyRepository(_context);

    // Transaction Management
    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        _transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
    }

    public async Task CommitTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_transaction != null)
        {
            await _transaction.CommitAsync(cancellationToken);
        }
    }

    public async Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_transaction != null)
        {
            await _transaction.RollbackAsync(cancellationToken);
        }
    }

    public void Dispose()
    {
        _transaction?.Dispose();
        GC.SuppressFinalize(this);
    }
}
