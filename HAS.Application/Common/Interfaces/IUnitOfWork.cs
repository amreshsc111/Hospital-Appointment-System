namespace HAS.Application.Common.Interfaces;

/// <summary>
/// Unit of Work pattern interface for coordinating repository operations and transaction management
/// </summary>
public interface IUnitOfWork : IDisposable
{
    // User & Authentication Repositories
    IUserRepository Users { get; }
    IRoleRepository Roles { get; }
    IRefreshTokenRepository RefreshTokens { get; }
    
    // Hospital Management Repositories
    IDepartmentRepository Departments { get; }
    IDoctorRepository Doctors { get; }
    IPatientRepository Patients { get; }
    IAppointmentRepository Appointments { get; }
    
    // Enhancement Repositories
    IDoctorScheduleRepository DoctorSchedules { get; }
    IDoctorLeaveRepository DoctorLeaves { get; }
    IAppointmentHistoryRepository AppointmentHistories { get; }
    IAppointmentReminderRepository AppointmentReminders { get; }
    ICancellationPolicyRepository CancellationPolicies { get; }
    
    // Transaction Management
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    Task BeginTransactionAsync(CancellationToken cancellationToken = default);
    Task CommitTransactionAsync(CancellationToken cancellationToken = default);
    Task RollbackTransactionAsync(CancellationToken cancellationToken = default);
}
