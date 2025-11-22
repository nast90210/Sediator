using System;

namespace Sediator.Abstractions
{
    public interface IHandlerProvider
    {
        public object GetHandler(Type handlerType);
    }
}