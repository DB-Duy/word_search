using Shared.Service.Audio.Internal;
using UnityEngine;

namespace Shared.Service.Audio
{
    public interface IAudioService
    {
        AudioSource PlaySound(string name, float time = 1, float pitch = 1f, float volume = 1);
        AudioSource PlayMusic(string name, float pitch = 1f, float volume = 1);
        AudioSource PlayButton();

        /// <summary>
        /// Stop them, and they can NOT be resumed. 
        /// </summary>
        void StopSounds();
        /// <summary>
        /// Pause it, and it CAN be resumed.
        /// </summary>
        void PauseMusic();

        void ResumeMusic();

        void RequestMute(MuteRequest r);
        void RemoveMuteRequest(MuteRequest r);
        
        void RequestMute(string name);
        void RemoveMuteRequest(string name);
    }
}