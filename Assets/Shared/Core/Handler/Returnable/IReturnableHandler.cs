namespace Shared.Core.Handler.Returnable
{
    public interface IReturnableHandler<R>
    {
        R Handle();
    }
    
    public interface IReturnableHandler<R, T>
    {
        R Handle(T t);
    }
    
    public interface IReturnableHandler<R, T1, T2>
    {
        R Handle(T1 t1, T2 t2);
    }
}