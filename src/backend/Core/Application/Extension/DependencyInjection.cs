using Application.Common.Dispatcher;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Application.Extension
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplicationDispatcher(this IServiceCollection services, Assembly? assembly = null)
        {
            assembly ??= typeof(NativeDispatcher).Assembly;
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
}
