using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Sediator.Abstractions;

namespace Sediator.DependencyInjection.Microsoft
{
    public static class SediatorServiceCollectionExtensions
    {
        public static IServiceCollection AddSediator(this IServiceCollection services)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            services.AddScoped<IHandlerProvider, HandlerProvider>();
            services.AddScoped<IMediator, Sediator>();

            return services;
        }

        public static IServiceCollection AddSediatorHandlers<T>(this IServiceCollection services)
        {
            return services == null ? throw new ArgumentNullException(nameof(services)) : AddSediatorHandlers(services, typeof(T).Assembly);
        }

        public static IServiceCollection AddSediatorHandlers(
            this IServiceCollection services,
            params Assembly[]? assemblies)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            if (assemblies == null || assemblies.Length == 0)
            {
                assemblies = [Assembly.GetCallingAssembly()];
            }

            foreach (var assembly in assemblies)
            {
                InternalAddHandlers(services, assembly, typeof(IRequestHandler<,>));
                InternalAddHandlers(services, assembly, typeof(IRequestHandler<>));
                InternalAddHandlers(services, assembly, typeof(INotificationHandler<>));
            }

            return services;
        }

        private static void InternalAddHandlers(IServiceCollection services, Assembly assembly, Type handlerType)
        {
            var handlers = assembly.GetTypes()
                .Where(t => !t.IsAbstract && !t.IsInterface)
                .Where(t => t.GetInterfaces()
                    .Any(i => i.IsGenericType &&
                        i.GetGenericTypeDefinition() == handlerType));

            foreach (var handler in handlers)
            {
                var handlerInterfaces = handler.GetInterfaces()
                    .Where(i => i.IsGenericType &&
                        i.GetGenericTypeDefinition() == handlerType);

                foreach (var handlerInterface in handlerInterfaces)
                {
                    services.AddScoped(handlerInterface, handler);
                }
            }
        }
    }
}