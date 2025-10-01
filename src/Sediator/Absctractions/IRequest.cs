namespace Sediator
{
    public interface IRequest<out TResponse> : IRequest { }
    public interface IRequest { }
}
