
    using UnityEngine;

    namespace Shared.Core.Repository.Audio
    {
        public interface IAudioClipRepository
        {
            AudioClip Get(string name);
        }
    }
