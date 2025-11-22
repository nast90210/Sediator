using Microsoft.Extensions.DependencyInjection;
using Sediator.Abstractions;

namespace Sediator.DependencyInjection.Microsoft
{
    public class HandlerProvider : IHandlerProvider
    {
        private readonly IServiceProvider _provider;

        public HandlerProvider(IServiceProvider provider)
        {
            _provider = provider ?? throw new ArgumentNullException(nameof(provider));
        }

        public object GetHandler(Type handlerType) => _provider.GetRequiredService(handlerType);
    }
}