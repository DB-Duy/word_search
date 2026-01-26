using System;

namespace Shared.Service.Ads
{
    public static class AdEvents
    {
        /// <summary>
        /// IronSourceInterstitialEvents.onAdOpenedEvent
        /// </summary>
        public static Action OnInterstitialShowStartedEvent = delegate { };
        /// <summary>
        /// IronSourceInterstitialEvents.onAdShowFailedEvent
        /// IronSourceInterstitialEvents.onAdClosedEvent
        /// </summary>
        public static Action OnInterstitialShowCompletedEvent = delegate { };
        
        public static Action OnInterstitialAdFakeLoadingStartedEvent = delegate { };
        public static Action OnInterstitialAdFakeLoadingCompletedEvent = delegate { };
        
        public static Action OnRewardedAdShowStartedEvent = delegate { };
        public static Action OnRewardedAdShowCompletedEvent = delegate { };
    }
}