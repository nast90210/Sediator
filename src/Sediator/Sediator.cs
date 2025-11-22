using System;
using System.Threading;
using System.Threading.Tasks;
using Sediator.Abstractions;
using Sediator.Handlers;

namespace Sediator
{
    public sealed class Sediator : IMediator
    {
        private readonly IHandlerProvider _provider;

        public Sediator(IHandlerProvider provider)
        {
            _provider = provider ?? throw new ArgumentNullException(nameof(provider));
        }

        public Task<TResponse> Send<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default)
        {
            var handlerType = typeof(IRequestHandler<,>).MakeGenericType(request.GetType(), typeof(TResponse));
            var handler = _provider.GetHandler(handlerType);
            return RequestHandler.Process(request, handlerType, handler, cancellationToken);
        }

        public Task Send(IRequest request, CancellationToken cancellationToken = default)
        {
            var handlerType = typeof(IRequestHandler<>).MakeGenericType(request.GetType());
            var handler = _provider.GetHandler(handlerType);
            return RequestHandler.Process(request, handlerType, handler, cancellationToken);
        }

        public Task Publish(INotification notification, CancellationToken cancellationToken = default)
        {
            var handlerType = typeof(INotificationHandler<>).MakeGenericType(notification.GetType());
            var handler = _provider.GetHandler(handlerType);
            return NotificationHandler.Process(notification, handlerType, handler, cancellationToken);
        }
    }
}