using Application.Common.Validation;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Application.Common.Dispatcher;

public class NativeDispatcher : IDispatcher
{
    private readonly IServiceProvider _serviceProvider;

    public NativeDispatcher(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task<TResponse> SendAsync<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default)
    {
        if (request == null)
            throw new ArgumentNullException(nameof(request));

        var requestType = request.GetType();

        // 1. Pipeline Behavior / Interceptor: Ejecutar validadores antes del handler
        var validatorType = typeof(IValidator<>).MakeGenericType(requestType);
        var validators = _serviceProvider.GetServices(validatorType);
        foreach (var validator in validators)
        {
            if (validator != null)
            {
                var validateMethod = validatorType.GetMethod(nameof(IValidator<object>.ValidateAsync));
                if (validateMethod != null)
                {
                    var validationTask = (Task)validateMethod.Invoke(validator, new object[] { request, cancellationToken })!;
                    await validationTask;
                }
            }
        }

        // 2. Resolver e invocar el Handler del caso de uso
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
