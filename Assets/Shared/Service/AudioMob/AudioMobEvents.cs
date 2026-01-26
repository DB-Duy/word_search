#if AUDIO_MOB
using System;
using Audiomob;

namespace Shared.Service.AudioMob
{
    public static class AudioMobEvents
    {
        // /// <summary>
        // /// (string placement, AdRequestResult adRequestResult, global::AudioMob.IAudioAd audioAd);
        // /// </summary>
        // public static Action<string, AdRequestResult, IAudioAd> OnAdRequestCompletedEvent = delegate { };
        // /// <summary>
        // /// (string placement, global::AudioMob.IAudioAd audioAd);
        // /// </summary>
        // public static Action<string, IAudioAd> OnAdPlaybackStartedEvent = delegate { };
        // /// <summary>
        // /// (string placement, AdPlaybackResult adPlaybackResult);
        // /// </summary>
        // public static Action<string, AdPlaybackStatus> OnAdPlaybackCompletedEvent = delegate { };
        // /// <summary>
        // /// (string placement);
        // /// </summary>
        // public static Action<string> OnSkipButtonAvailableEvent = delegate { };
        
        // new Audiomob Unity Plugin v10 Upgrade
        public static Action<string, AdSequence, AdPlaybackStatus> OnAdPlaybackStatusChangedEvent = delegate { };
        
        public static Action<string, AdSequence, IAudioAd> OnAdPaidEvent = delegate { };

        public static Action<string, AdSequence> OnAdClickedEvent = delegate { };
        
        public static Action<string, AdSequence, string> OnAdFailedEvent = delegate { };
    }
}
#endif