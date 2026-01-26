using System;
using System.Collections.Generic;
using UnityEngine;

namespace Shared.Core.Repository.Prefab
{
    public class CacheFeaturePrefabRepository : FeaturePrefabRepository
    {
        private readonly Dictionary<string, GameObject> _prefabCache = new();
        
        public override T GetOrLoad<T>()
        {
            var fullName = typeof(T).FullName;
            if (string.IsNullOrEmpty(fullName)) 
                throw new ArgumentException($"string.IsNullOrEmpty(fullName) for {typeof(T)}");
            if (_prefabCache.TryGetValue(fullName, out var value)) return value.GetComponent<T>();
            var t = base.GetOrLoad<T>();
            _prefabCache.Add(fullName, t.gameObject);
            return t;
        }
    }
}