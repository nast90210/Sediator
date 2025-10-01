using System.Threading;
using System.Threading.Tasks;

namespace Sediator;

public interface IMediator
{
    Task<TResponse> Send<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default);
    Task Send(IRequest request, CancellationToken cancellationToken = default);

    Task Publish(INotification notification, CancellationToken cancellationToken = default);
}
