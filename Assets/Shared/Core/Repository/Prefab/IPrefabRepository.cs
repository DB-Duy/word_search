using UnityEngine;

namespace Shared.Core.Repository.Prefab
{
    public interface IPrefabRepository
    {
        T GetOrLoad<T>() where T : MonoBehaviour;
        void Unload<T>() where T : MonoBehaviour;
    }
}