namespace Shared.Core.Handler
{
    public interface IHandler
    {
        void Handle();
    }
    
    public interface IHandler<T>
    {
        void Handle(T t);
    }
    
    public interface IHandler<T, R>
    {
        void Handle(T t, R r);
    }
}