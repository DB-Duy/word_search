namespace Shared.Core.ObjectPool
{
    public interface IPool<T>
    {
        T Get();
        void Release(T t);
    }
    
    public interface IPool<T, K>
    {
        T Get(K k);
        void Release(T t);
    }
}