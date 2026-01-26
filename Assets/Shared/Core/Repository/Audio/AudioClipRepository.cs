using Shared.Core.IoC;
using UnityEngine;

namespace Shared.Core.Repository.Audio
{
    [Repository]
    public class AudioClipRepository : IAudioClipRepository
    {
        private readonly string _prefixPath;

        public AudioClipRepository(string prefixPath)
        {
            _prefixPath = prefixPath;
            // Remove trailing slash if it exists
            if (_prefixPath.EndsWith("/"))
            {
                _prefixPath = _prefixPath.TrimEnd('/');
            }
        }

        public virtual AudioClip Get(string name)
        {
            var path = string.IsNullOrEmpty(_prefixPath) ? $"{name}" : $"{_prefixPath}/{name}";
            return Resources.Load<AudioClip>(path);
        }
    }
}