using HAS.Domain.Common.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace HAS.Infrastructure.Identity;

public static class AuthInfrastructure
{
    //Register authentication services in DI
    public static IServiceCollection AddAuthInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        // Password hasher
        services.AddSingleton<IPasswordHasher, PasswordHasher>();

        // JWT token service
        services.AddSingleton<IJwtTokenService, JwtTokenService>();

        //Refresh token service
        services.AddSingleton<IRefreshTokenService, RefreshTokenService>();

        return services;
    }
}
