#if FACEBOOK_INSTANT
using System;
using Shared.Utils;
using UnityEngine;

namespace Shared.Ads.FacebookInstant.Callbacks
{
    /// <summary>
    /// https://docs.unity3d.com/Manual/webgl-interactingwithbrowserscripting.html
    /// </summary>
    [DisallowMultipleComponent]
    public class FacebookInstantEvents : MonoBehaviour
    {
        // https://developers.facebook.com/docs/games/monetize/banner-ads
        public static class Banner
        {
            // Load
            public static Action onAdLoadedEvent = delegate { };
            public static Action<string> onAdLoadFailedEvent = delegate { };
        }
        
        public static class Interstitial
        {
            // Load
            public static Action onAdReadyEvent = delegate {  };
            public static Action<string> onAdLoadFailedEvent = delegate { };
            // Show
            public static Action onAdShowSucceededEvent = delegate { };
            public static Action<string> onAdShowFailedEvent = delegate { };
            // Close
            public static Action onAdClosedEvent = delegate { };
        }
        
        public static class Reward
        {
            // Load
            public static Action onAdReadyEvent = delegate {  };
            public static Action<string> onAdLoadFailedEvent = delegate {  };
            // Show
            public static Action onAdOpenedEvent = delegate {  };
            public static Action<string> onAdShowFailedEvent = delegate {  };
            // Close
            public static Action onAdClosedEvent = delegate {  };
            // Reward
            public static Action onAdRewardedEvent = delegate {  };
        }

        // ReSharper disable once InconsistentNaming
        private const string TAG = "FacebookInstantEvents";
        
        private static FacebookInstantEvents _instance;
        public static FacebookInstantEvents DefaultInstance
        {
            get
            {
                if (_instance != null) return _instance;
                var go = new GameObject(TAG);
                _instance = go.AddComponent<FacebookInstantEvents>();
                GameObject.DontDestroyOnLoad(go);
                SharedLogger.Log($"{TAG}->DefaultInstance: {go.name}");
                return _instance;
            }
        }

        public void Create()
        {
            SharedLogger.Log($"{TAG}->Create");
        }

        // ---------------------------------------------------------------------------------
        // Banner
        // ---------------------------------------------------------------------------------
        public void OnBannerAdLoadedEvent()
        {
            SharedLogger.Log($"{TAG}->OnBannerAdLoadedEvent");
            Banner.onAdLoadedEvent.Invoke();
        }

        public void OnBannerAdLoadFailedEvent(string message)
        {
            SharedLogger.Log($"{TAG}->OnBannerAdLoadFailedEvent");
            Banner.onAdLoadFailedEvent.Invoke(message);
        }
        // ---------------------------------------------------------------------------------
        // Interstitial
        // ---------------------------------------------------------------------------------
        public void OnInterstitialAdReadyEvent()
        {
            SharedLogger.Log($"{TAG}->OnInterstitialAdReadyEvent");
            Interstitial.onAdReadyEvent.Invoke();
        }
        
        public void OnInterstitialAdLoadFailedEvent(string message)
        {
            SharedLogger.Log($"{TAG}->OnInterstitialAdLoadFailedEvent");
            Interstitial.onAdLoadFailedEvent.Invoke(message);
        }
        
        public void OnInterstitialAdShowSucceededEvent()
        {
            SharedLogger.Log($"{TAG}->OnInterstitialAdShowSucceededEvent");
            Interstitial.onAdShowSucceededEvent.Invoke();
        }
        
        public void OnInterstitialAdShowFailedEvent(string message)
        {
            SharedLogger.Log($"{TAG}->OnInterstitialAdShowFailedEvent");
            Interstitial.onAdShowFailedEvent.Invoke(message);
        }
        
        public void OnInterstitialAdClosedEvent()
        {
            SharedLogger.Log($"{TAG}->OnInterstitialAdClosedEvent");
            Interstitial.onAdClosedEvent.Invoke();
        }
        
        // ---------------------------------------------------------------------------------
        // Reward
        // ---------------------------------------------------------------------------------
        public void OnRewardAdReadyEvent()
        {
            SharedLogger.Log($"{TAG}->OnRewardAdReadyEvent");
            Reward.onAdReadyEvent.Invoke();
        }
        
        public void OnRewardAdLoadFailedEvent(string message)
        {
            SharedLogger.Log($"{TAG}->OnRewardAdLoadFailedEvent");
            Reward.onAdLoadFailedEvent.Invoke(message);
        }
        
        public void OnRewardAdOpenedEvent()
        {
            SharedLogger.Log($"{TAG}->OnRewardAdOpenedEvent");
            Reward.onAdOpenedEvent.Invoke();
        }
        
        public void OnRewardAdClosedEvent()
        {
            SharedLogger.Log($"{TAG}->OnRewardAdClosedEvent");
            Reward.onAdClosedEvent.Invoke();
        }
        
        public void OnRewardAdShowFailedEvent(string message)
        {
            SharedLogger.Log($"{TAG}->OnRewardAdShowFailedEvent: {message}");
            Reward.onAdShowFailedEvent.Invoke(message);
        }
        
        public void OnRewardAdRewardedEvent()
        {
            SharedLogger.Log($"{TAG}->OnRewardAdRewardedEvent");
            Reward.onAdRewardedEvent.Invoke();
        }
    }
}
#endif