namespace Sediator.Abstractions
{
    public interface IRequest<out TResponse> : IRequest { }
    public interface IRequest { }
}
