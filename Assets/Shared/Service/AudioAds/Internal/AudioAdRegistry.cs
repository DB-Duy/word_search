using System;
using System.Collections.Generic;
using Shared.Utils;
using Shared.View.AudioAds;

namespace Shared.Service.AudioAds.Internal
{
    public static class AudioAdRegistry
    {
        private const string Tag = "AudioAdRegistration";

        public static Action<AudioAdPlacement> OnNewPlacementAddedEvent = delegate { };
        public static Action<AudioAdPlacement> OnPlacementRemovedEvent = delegate { };

        public static HashSet<ISharedAudioAd> AudioAds { get; } = new();
        public static List<AudioAdPlacement> PlacementStack { get; } = new();

        public static void Register(AudioAdPlacement placement)
        {
            PlacementStack.Remove(placement);
            PlacementStack.Insert(0, placement);
            OnNewPlacementAddedEvent?.Invoke(placement);
        }

        public static void Remove(AudioAdPlacement placement)
        {
            PlacementStack.Remove(placement);
            OnPlacementRemovedEvent?.Invoke(placement);
        }

        public static void Register(ISharedAudioAd ad)
        {
            SharedLogger.LogJson(SharedLogTag.AudioAds, $"{Tag}->Register", nameof(ad), ad.GetType().FullName);
            AudioAds.Add(ad);
        }

        public static void Remove(ISharedAudioAd ad)
        {
            SharedLogger.LogJson(SharedLogTag.AudioAds, $"{Tag}->Remove", nameof(ad), ad.GetType().FullName);
            AudioAds.Remove(ad);
        }
    }
}