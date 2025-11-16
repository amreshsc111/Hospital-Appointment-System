namespace HAS.Application.Common.Interfaces;

public interface IMediator
{
    Task<TResponse> Send<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default);
}

public class Mediator(IServiceProvider serviceProvider) : IMediator
{
    private readonly IServiceProvider _serviceProvider = serviceProvider;

    public async Task<TResponse> Send<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default)
    {
        var handlerType = typeof(IRequestHandler<,>).MakeGenericType(request.GetType(), typeof(TResponse));
        var handler = _serviceProvider.GetService(handlerType) ?? throw new InvalidOperationException($"No handler registered for {request.GetType().FullName}");

        // Build pipeline behaviors
        var behaviorType = typeof(IPipelineBehavior<,>).MakeGenericType(request.GetType(), typeof(TResponse));
        var behaviors = (IEnumerable<object>)_serviceProvider.GetService(typeof(IEnumerable<>).MakeGenericType(behaviorType)) ?? Enumerable.Empty<object>();

        // Final delegate that invokes handler
        RequestHandlerDelegate<TResponse> handlerDelegate = () => (Task<TResponse>)handlerType.GetMethod("Handle")!.Invoke(handler, new object[] { request, cancellationToken })!;

        // Wrap behaviors around the handler delegate (reverse order)
        var pipeline = behaviors.Reverse().Aggregate(handlerDelegate, (next, behavior) =>
        {
            return () => (Task<TResponse>)behaviorType.GetMethod("Handle")!.Invoke(behavior, new object[] { request, cancellationToken, next })!;
        });

        return await pipeline();
    }
}
