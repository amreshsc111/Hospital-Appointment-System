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
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
        services.AddScoped<IRoleRepository, RoleRepository>();
        
        // Register Hospital Management repositories
        services.AddScoped<IDepartmentRepository, DepartmentRepository>();
        services.AddScoped<IDoctorRepository, DoctorRepository>();
        services.AddScoped<IPatientRepository, PatientRepository>();
        services.AddScoped<IAppointmentRepository, AppointmentRepository>();
        
        // Register Enhancement repositories
        services.AddScoped<IAppointmentHistoryRepository, AppointmentHistoryRepository>();
        services.AddScoped<IDoctorScheduleRepository, DoctorScheduleRepository>();
        services.AddScoped<IDoctorLeaveRepository, DoctorLeaveRepository>();
        services.AddScoped<IAppointmentReminderRepository, AppointmentReminderRepository>();
        services.AddScoped<ICancellationPolicyRepository, CancellationPolicyRepository>();
        
        // Register Services
        services.AddScoped<IEmailService, Services.EmailService>();
        services.AddScoped<ICancellationPolicyService, Services.CancellationPolicyService>();

        return services;
    }
}
