using System;
using System.Threading;
using System.Threading.Tasks;

namespace Sediator.Handlers;

internal static class NotificationHandler
{
    private const string _handlerMethodName = "Handle";

    internal static async Task Process(
        INotification notification,
        Type handlerType,
        object handler,
        CancellationToken cancellationToken)
    {
        var handleMethod = handlerType.GetMethod(_handlerMethodName);
        var task = (Task)handleMethod.Invoke(handler, [notification, cancellationToken]);
        await task.ConfigureAwait(false);
    }
}
