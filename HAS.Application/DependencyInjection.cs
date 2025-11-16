using HAS.Application.Common;
using HAS.Application.Common.Interfaces;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using FluentValidation;

namespace HAS.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        // Register Mediator
        services.AddSingleton<IMediator, Mediator>();


        // Register all IRequestHandler<,> implementations from this assembly
        var asm = Assembly.GetExecutingAssembly();
        var handlerInterfaceType = typeof(IRequestHandler<,>);


        var handlerTypes = asm.GetTypes()
        .Where(t => !t.IsAbstract && !t.IsInterface)
        .SelectMany(t => t.GetInterfaces(), (impl, i) => new { impl, i })
        .Where(x => x.i.IsGenericType && x.i.GetGenericTypeDefinition() == handlerInterfaceType)
        .Select(x => new { Service = x.i, Implementation = x.impl })
        .ToList();

        foreach (var h in handlerTypes)
        {
            services.AddTransient(h.Service, h.Implementation);
        }

        // Register pipeline behaviors (Validation)
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

        // Register FluentValidation validators in this assembly
        services.AddValidatorsFromAssembly(asm);

        return services;
    }
}
