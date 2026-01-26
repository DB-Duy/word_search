using UnityEngine;

namespace Shared.ObjectPool
{
    public interface IObjectPool
    {
        GameObject Get();
        T Get<T>();
        void Release(GameObject go);
    }
}