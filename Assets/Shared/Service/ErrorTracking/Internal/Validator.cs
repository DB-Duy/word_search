using System.Collections.Generic;
using UnityEngine;

namespace Shared.Tracking.ErrorTracking
{
    public class Validator : IValidator
    {
        private const float Interval = 0.5f;
        private const float CleanInterval = 2f;
        
        private readonly Dictionary<string, float> _cache = new();
        public bool Validate(string log)
        {
            _CleanCache();
            if (_cache.ContainsKey(log))
            {
                var interval = Time.realtimeSinceStartup - _cache[log];
                if (!(interval >= Interval)) return false;
                _cache[log] = Time.realtimeSinceStartup;
                return true;
            }

            _cache.Add(log, Time.realtimeSinceStartup);
            return true;
        }

        private void _CleanCache()
        {
            if (_cache.Count == 0) return;
            var keys = new List<string>(_cache.Keys);
            foreach (var k in keys)
            {
                var interval = Time.realtimeSinceStartup - _cache[k];
                if (interval >= CleanInterval)
                {
                    _cache.Remove(k);
                }
            }
        }
    }
}