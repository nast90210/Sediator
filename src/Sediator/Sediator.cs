using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Sediator.Handlers;

namespace Sediator
{
    public sealed class Sediator : IMediator
    {
        private readonly IServiceProvider _provider;

        public Sediator(IServiceProvider provider)
        {
            _provider = provider ?? throw new ArgumentNullException(nameof(provider));
        }

        public Task<TResponse> Send<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default)
        {
            var handlerType = typeof(IRequestHandler<,>).MakeGenericType(request.GetType(), typeof(TResponse));
            var handler = _provider.GetRequiredService(
                    typeof(IRequestHandler<,>).MakeGenericType(request.GetType(), typeof(TResponse)));
            return RequestHandler.Process(request, handlerType, handler, cancellationToken);
        }

        public Task Send(IRequest request, CancellationToken cancellationToken = default)
        {
            var handlerType = typeof(IRequestHandler<>).MakeGenericType(request.GetType());
            var handler = _provider.GetRequiredService(
                    typeof(IRequestHandler<>).MakeGenericType(request.GetType()));
            return RequestHandler.Process(request, handlerType, handler, cancellationToken);
        }

        public Task Publish(INotification notification, CancellationToken cancellationToken = default)
        {
            var handlerType = typeof(INotificationHandler<>).MakeGenericType(notification.GetType());
            var handler = _provider.GetRequiredService(
                    typeof(INotificationHandler<>).MakeGenericType(notification.GetType()));
            return NotificationHandler.Process(notification, handlerType, handler, cancellationToken);
        }
    }
}