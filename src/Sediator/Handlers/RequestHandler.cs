using System;
using System.Threading;
using System.Threading.Tasks;
using Sediator.Abstractions;

namespace Sediator.Handlers
{
    internal static class RequestHandler
    {
        private const string HandlerMethodName = "Handle";
        private const string ResultPropertyName = "Result";

        internal static async Task<TResponse> Process<TResponse>(
            IRequest<TResponse> request,
            Type handlerType,
            object handler,
            CancellationToken token = default)
        {
            var handleMethod = handlerType.GetMethod(HandlerMethodName);
            var task = (Task<TResponse>)handleMethod!.Invoke(handler, [request, token])!;
            await task.ConfigureAwait(false);
            var resultProperty = task.GetType().GetProperty(ResultPropertyName);
            var result = resultProperty!.GetValue(task);
            return (TResponse)result!;
        }

        internal static async Task Process(
            IRequest request,
            Type handlerType,
            object handler,
            CancellationToken token = default)
        {
            var handleMethod = handlerType.GetMethod(HandlerMethodName);
            var task = (Task)handleMethod!.Invoke(handler, [request, token])!;
            await task.ConfigureAwait(false);
        }
    }
}
