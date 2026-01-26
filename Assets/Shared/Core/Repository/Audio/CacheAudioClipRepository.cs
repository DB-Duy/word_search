using System.Collections.Generic;
using Shared.Core.IoC;
using UnityEngine;

namespace Shared.Core.Repository.Audio
{
    [Repository]
    public class CacheAudioClipRepository : AudioClipRepository
    {
        private readonly Dictionary<string, AudioClip> _cache = new();
        
        public CacheAudioClipRepository(string prefixPath = "") : base(prefixPath)
        {
        }
        
        public override AudioClip Get(string name)
        {
            if (_cache.TryGetValue(name, out var value)) return value;
            var o = base.Get(name);
            _cache.Add(name, o);
            return o;
        }
    }
}