using HAS.Domain.Common.Interfaces;
using HAS.Domain.Enums;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace HAS.Infrastructure.Identity;

public static class AuthInfrastructure
{
    //Register authentication services in DI
    public static IServiceCollection AddAuthInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        // Password hasher (stateless, can be singleton)
        services.AddSingleton<IPasswordHasher, PasswordHasher>();

        // JWT token service (stateless, can be singleton)
        services.AddSingleton<IJwtTokenService, JwtTokenService>();

        // Refresh token service (depends on DbContext, must be scoped)
        services.AddScoped<IRefreshTokenService, RefreshTokenService>();

        return services;
    }

    public static IServiceCollection AddAuthorizationPolicies(this IServiceCollection services)
    {
        services.AddAuthorization(options =>
        {
            options.AddPolicy(AuthorizationPolicies.AdminOnly, policy => 
                policy.RequireClaim(AuthorizationPolicies.RoleClaimType, UserRole.Admin.ToString()));
                
            options.AddPolicy(AuthorizationPolicies.DoctorOnly, policy => 
                policy.RequireClaim(AuthorizationPolicies.RoleClaimType, UserRole.Doctor.ToString()));
                
            options.AddPolicy(AuthorizationPolicies.PatientOnly, policy => 
                policy.RequireClaim(AuthorizationPolicies.RoleClaimType, UserRole.Patient.ToString()));
                
            options.AddPolicy(AuthorizationPolicies.ReceptionistOnly, policy => 
                policy.RequireClaim(AuthorizationPolicies.RoleClaimType, UserRole.Receptionist.ToString()));
                
            options.AddPolicy(AuthorizationPolicies.AdminOrReceptionist, policy => 
                policy.RequireAssertion(context => 
                    context.User.HasClaim(c => c.Type == AuthorizationPolicies.RoleClaimType && 
                                              (c.Value == UserRole.Admin.ToString() || c.Value == UserRole.Receptionist.ToString()))));
                                              
            options.AddPolicy(AuthorizationPolicies.DoctorOrReceptionist, policy => 
                policy.RequireAssertion(context => 
                    context.User.HasClaim(c => c.Type == AuthorizationPolicies.RoleClaimType && 
                                              (c.Value == UserRole.Doctor.ToString() || c.Value == UserRole.Receptionist.ToString()))));
                                              
            options.AddPolicy(AuthorizationPolicies.AdminOrDoctor, policy => 
                policy.RequireAssertion(context => 
                    context.User.HasClaim(c => c.Type == AuthorizationPolicies.RoleClaimType && 
                                              (c.Value == UserRole.Admin.ToString() || c.Value == UserRole.Doctor.ToString()))));
        });

        return services;
    }
}
