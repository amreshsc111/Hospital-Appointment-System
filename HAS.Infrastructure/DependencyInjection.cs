using HAS.Domain.Common.Interfaces;
using HAS.Infrastructure.Identity;
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

        return services;
    }
}
