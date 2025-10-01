using System.Threading.Tasks;

namespace Sediator
{
    public delegate Task<TResponse> RequestDelegate<TResponse>();
}