using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Shared.Core.Repository.Prefab
{
    public class MonoBehaviourPool<K> : CacheFeaturePrefabRepository
    {
        private readonly Dictionary<string, List<MonoBehaviour>> _poolDict = new();
        
        public T GetObject<T>() where T : MonoBehaviour, K
        {
            var fullName = typeof(T).FullName;
            if (string.IsNullOrEmpty(fullName)) 
                throw new ArgumentException($"string.IsNullOrEmpty(fullName) for {typeof(T)}");
            T t = null;
            if (!_poolDict.ContainsKey(fullName) || _poolDict[fullName].Count == 0)
            {
                var prefab = GetOrLoad<T>();
                t = Object.Instantiate(prefab);
            }
            else
            {
                t = (T)_poolDict[fullName][0];
                _poolDict[fullName].RemoveAt(0);
            }
            return t;
        }

        public void ReleaseObject(MonoBehaviour t)
        {
            var fullName = t.GetType().FullName;
            if (string.IsNullOrEmpty(fullName)) 
                throw new ArgumentException($"string.IsNullOrEmpty(fullName) for {t}");
            t.gameObject.SetActive(false);
            _Ensure(fullName);
            _poolDict[fullName].Add(t);
        }

        private void _Ensure(string fullName)
        {
            if (_poolDict.ContainsKey(fullName)) return;
            _poolDict.Add(fullName, new List<MonoBehaviour>());
        }
    }
}