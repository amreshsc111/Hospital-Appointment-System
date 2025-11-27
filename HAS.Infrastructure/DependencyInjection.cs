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

        return services;
    }
}
