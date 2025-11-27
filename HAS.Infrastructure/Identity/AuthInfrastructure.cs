using HAS.Domain.Common.Interfaces;
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
}
