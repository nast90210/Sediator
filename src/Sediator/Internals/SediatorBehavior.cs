using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace Sediator.Internals;

public class SediatorBehavior
{
    
        private IEnumerable<IPipelineBehavior<TRequest, TResponse>> GetPipelineBehaviors<TRequest, TResponse>(
            TRequest request,
            IServiceScope scope)
            where TRequest : IRequest<TResponse>
        {
            return scope.ServiceProvider.GetServices<IPipelineBehavior<TRequest, TResponse>>();
        }

        private object[] GetPipelineBehaviors(object request, IServiceScope scope)
        {
            var requestType = request.GetType();
            var responseType = requestType.GetInterfaces()
                .First(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IRequest<>))
                .GetGenericArguments()[0];

            var behaviorType = typeof(IPipelineBehavior<,>).MakeGenericType(requestType, responseType);
            return scope.ServiceProvider.GetServices(behaviorType).Cast<object>().ToArray();
        }

        private async Task<object> ExecutePipeline(
            object request,
            object[] behaviors,
            Func<Task<object>> handler,
            CancellationToken cancellationToken)
        {
            if (behaviors == null || behaviors.Length == 0)
                return await handler();

            var index = 0;
            var requestType = request.GetType();
            try
            {
                var t = requestType.GetInterfaces();
                var a = t.First(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IRequest<>));
                var r = a.GetGenericArguments()[0];

                var responseType1 = requestType.GetInterfaces()
                .First(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IRequest<>))
                .GetGenericArguments()[0];
            }
            catch (Exception ex)
            {
                var msg = ex.Message;
            }
            var responseType = requestType.GetInterfaces()
                .First(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IRequest<>))
                .GetGenericArguments()[0];

            async Task<object> Next()
            {
                if (index == behaviors.Length)
                    return await handler();

                var behavior = behaviors[index++];
                var handleMethod = behavior.GetType().GetMethod("Handle");

                // Создаем правильный делегат
                var nextFunc = new RequestDelegate<object>(Next);

                // Создаем правильно типизированный делегат для конкретного типа ответа
                var delegateType = typeof(RequestDelegate<>).MakeGenericType(responseType);
                var convertedDelegate = Delegate.CreateDelegate(
                    delegateType,
                    nextFunc.Target,
                    nextFunc.Method);

                var result = await (Task<object>)handleMethod.Invoke(
                    behavior,
                    new[] { request, convertedDelegate, cancellationToken });

                return result;
            }

            return await Next();
        }
}
