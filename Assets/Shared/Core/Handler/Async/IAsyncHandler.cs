using Shared.Core.Async;

namespace Shared.Core.Handler.Async
{
    public interface IAsyncHandler
    {
        IAsyncOperation Handle();
    }
    
    public interface IAsyncHandler<T>
    {
        IAsyncOperation Handle(T apsSlotId);
    }
    
    public interface IAsyncHandler<R, T>
    {
        IAsyncOperation<R> Handle(T t);
    }
}