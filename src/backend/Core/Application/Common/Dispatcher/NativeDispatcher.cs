using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Application.Common.Dispatcher;

public class NativeDispatcher : IDispatcher
{
    private readonly IServiceProvider _serviceProvider;

    public NativeDispatcher(IServiceProvider _serviceProvider)
    {
        this._serviceProvider = _serviceProvider;
    }

    public async Task<TResponse> SendAsync<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default)
    {
        if (request == null)
            throw new ArgumentNullException(nameof(request));

        var requestType = request.GetType();
        var handlerType = typeof(IRequestHandler<,>).MakeGenericType(requestType, typeof(TResponse));

        var handler = _serviceProvider.GetService(handlerType);
        if (handler == null)
            throw new InvalidOperationException($"No se encontró ningún manejador registrado para la solicitud '{requestType.Name}'.");

        var method = handlerType.GetMethod(nameof(IRequestHandler<IRequest<TResponse>, TResponse>.HandleAsync));
        if (method == null)
            throw new InvalidOperationException($"No se pudo invocar el método HandleAsync para '{handlerType.Name}'.");

        var task = (Task<TResponse>)method.Invoke(handler, new object[] { request, cancellationToken })!;
        return await task;
    }
}

public static class DispatcherExtensions
{
    public static IServiceCollection AddApplicationDispatcher(this IServiceCollection services, Assembly assembly)
    {
        services.AddScoped<IDispatcher, NativeDispatcher>();

        var handlerInterfaceType = typeof(IRequestHandler<,>);
        var handlerTypes = assembly.GetTypes()
            .Where(t => !t.IsAbstract && !t.IsInterface)
            .SelectMany(t => t.GetInterfaces(), (t, i) => new { Implementation = t, Interface = i })
            .Where(x => x.Interface.IsGenericType && x.Interface.GetGenericTypeDefinition() == handlerInterfaceType);

        foreach (var item in handlerTypes)
        {
            services.AddScoped(item.Interface, item.Implementation);
        }

        return services;
    }
}
