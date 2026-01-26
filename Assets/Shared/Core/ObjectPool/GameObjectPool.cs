using System.Collections.Generic;
using UnityEngine;

namespace Shared.ObjectPool
{
    [DisallowMultipleComponent]
    public class GameObjectPool : MonoBehaviour, IObjectPool
    {
        [SerializeField] private Transform myTransform;
        [SerializeField] private GameObject prefab;
        private readonly List<GameObject> _pool = new();
        
        
        public GameObject Get()
        {
            if (_pool.Count <= 0) return Instantiate(prefab);
            var i = _pool[0];
            _pool.RemoveAt(0);
            return i;
        }

        public T Get<T>() => Get().GetComponent<T>();

        public void Release(GameObject go)
        {
            go.transform.SetParent(myTransform);
            go.SetActive(false);
            _pool.Add(go);
        }
    }
}