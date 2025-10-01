using System;
using System.Threading;
using System.Threading.Tasks;

namespace Sediator.Handlers
{
    internal static class RequestHandler
    {
        private const string _handlerMethodName = "Handle";
        private const string _resultPropertyName = "Result";

        internal async static Task<TResponse> Process<TResponse>(
            IRequest<TResponse> request,
            Type handlerType,
            object handler,
            CancellationToken token = default)
        {
            var handleMethod = handlerType.GetMethod(_handlerMethodName);
            var task = (Task<TResponse>)handleMethod!.Invoke(handler, [request, token])!;
            await task.ConfigureAwait(false);
            var resultProperty = task.GetType().GetProperty(_resultPropertyName);
            var result = resultProperty!.GetValue(task);
            return (TResponse)result!;
        }

        internal async static Task Process(
            IRequest request,
            Type handlerType,
            object handler,
            CancellationToken token = default)
        {
            var handleMethod = handlerType.GetMethod(_handlerMethodName);
            var task = (Task)handleMethod!.Invoke(handler, [request, token])!;
            await task.ConfigureAwait(false);
        }

    }
}
