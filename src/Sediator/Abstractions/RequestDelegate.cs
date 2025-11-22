using System.Threading.Tasks;

namespace Sediator.Abstractions
{
    public delegate Task<TResponse> RequestDelegate<TResponse>();
}