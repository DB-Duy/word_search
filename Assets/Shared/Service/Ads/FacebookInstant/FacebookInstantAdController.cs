#if FACEBOOK_INSTANT
using Shared.Ads.FacebookInstant.Banner;
using Shared.Ads.FacebookInstant.Callbacks;
using Shared.Ads.FacebookInstant.Interstitial;
using Shared.Ads.FacebookInstant.Reward;
using Shared.Common;
using Shared.Utils;
using UnityEngine;

namespace Shared.Ads.FacebookInstant
{
    public class FacebookInstantAdController : MonoBehaviour, IAdController
    {
        // ReSharper disable once InconsistentNaming
        private const string TAG = "FacebookInstantAdController";
        
        public IAdConfig AdConfig { get; private set; }
        
        public IBannerAd BannerAd { get; private set; }
        public IInterstitialAd InterstitialAd { get; private set;}
        public IRewardAd RewardAd { get; private set;}
        
        public bool IsInitialized => true;
        public bool IsNotInitialized => false;

        private IAsyncOperation _initOperation;
        
        public IAdController SetUp(IAdConfig adConfig)
        {
            AdConfig = adConfig;
            BannerAd = gameObject.AddComponent<FacebookInstantBannerAd>();
            InterstitialAd = gameObject.AddComponent<FacebookInstantInterstitialAd>();
            RewardAd = gameObject.AddComponent<FacebookInstantRewardedAd>();
            return this;
        }

        public IAsyncOperation Initialize()
        {
            if (_initOperation != null) return _initOperation;
            FacebookInstantEvents.DefaultInstance.Create();
            BannerAd.Setup(this);
            InterstitialAd.Setup(this);
            RewardAd.Setup(this);
            _initOperation = new SharedAsyncOperation().Success();
            return _initOperation;
        }
        
        public void LaunchTestSuite()
        {
            SharedLogger.Log($"{TAG}->LaunchTestSuite");
        }
    }
}
#endif