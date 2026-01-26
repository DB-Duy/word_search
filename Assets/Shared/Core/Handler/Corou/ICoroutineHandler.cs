using System.Collections;

namespace Shared.Core.Handler.Corou
{
    public interface ICoroutineHandler
    {
        IEnumerator Handle();
    }
    
    public interface ICoroutineHandler<T>
    {
        IEnumerator Handle(T t);
    }
    
    public interface IReturnableCoroutineHandler<T>
    {
        System.Collections.Generic.IEnumerator<T> Handle();
    }
}