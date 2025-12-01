using HAS.Application.Common.Interfaces;
using HAS.Domain.Common.Interfaces;
using HAS.Infrastructure.Identity;
using HAS.Infrastructure.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace HAS.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services)
    {
        // IHttpContextAccessor needed for ICurrentUserService
        services.AddHttpContextAccessor();

        // Register CurrentUserService for auditing purposes
        services.AddScoped<ICurrentUserService, CurrentUserService>();
        
        // Register Unit of Work (replaces individual repository registrations)
        // Register Unit of Work
        services.AddScoped<IUnitOfWork, Persistence.UnitOfWork>();

        // Register Repositories (Forward to UnitOfWork to ensure same instance/context)
        services.AddScoped<IUserRepository>(sp => sp.GetRequiredService<IUnitOfWork>().Users);
        services.AddScoped<IRoleRepository>(sp => sp.GetRequiredService<IUnitOfWork>().Roles);
        services.AddScoped<IRefreshTokenRepository>(sp => sp.GetRequiredService<IUnitOfWork>().RefreshTokens);
        services.AddScoped<IDepartmentRepository>(sp => sp.GetRequiredService<IUnitOfWork>().Departments);
        services.AddScoped<IDoctorRepository>(sp => sp.GetRequiredService<IUnitOfWork>().Doctors);
        services.AddScoped<IPatientRepository>(sp => sp.GetRequiredService<IUnitOfWork>().Patients);
        services.AddScoped<IAppointmentRepository>(sp => sp.GetRequiredService<IUnitOfWork>().Appointments);
        services.AddScoped<IDoctorScheduleRepository>(sp => sp.GetRequiredService<IUnitOfWork>().DoctorSchedules);
        services.AddScoped<IDoctorLeaveRepository>(sp => sp.GetRequiredService<IUnitOfWork>().DoctorLeaves);
        services.AddScoped<IAppointmentHistoryRepository>(sp => sp.GetRequiredService<IUnitOfWork>().AppointmentHistories);
        services.AddScoped<IAppointmentReminderRepository>(sp => sp.GetRequiredService<IUnitOfWork>().AppointmentReminders);
        services.AddScoped<ICancellationPolicyRepository>(sp => sp.GetRequiredService<IUnitOfWork>().CancellationPolicies);
        
        // Register Services
        services.AddScoped<IEmailService, Services.EmailService>();
        services.AddScoped<ICancellationPolicyService, Services.CancellationPolicyService>();

        return services;
    }
}
