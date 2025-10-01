using System.Threading;
using System.Threading.Tasks;

namespace Sediator
{
    public interface IPipelineBehavior<in TRequest, TResponse>
        where TRequest : IRequest<TResponse>
    {
        Task<TResponse> Handle(
            TRequest request,
            RequestDelegate<TResponse> next,
            CancellationToken cancellationToken);
    }
}