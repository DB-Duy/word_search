#if UNITY_EDITOR
using System;
using Shared.Core.Async;
using Shared.Core.IoC;
using Shared.Utils;
using Zenject;

namespace Shared.Service.Ads.Fake
{
    [Service]
    public class FakeAdService : IAdService
    {
        private const string Tag = "FakeAdService";
        
        [Inject] private IRewardAd _rewardAd;
        [Inject] private IInterstitialAd _interstitialAd;
        [Inject] private IBannerAd _bannerAd;

        public IBannerAd BannerAd => _bannerAd;
        public IInterstitialAd InterstitialAd => _interstitialAd;
        public IRewardAd RewardAd => _rewardAd;
        
        public bool IsInitialized { get; private set; }
        private IAsyncOperation _initOperation;
        
        public IAsyncOperation Initialize()
        {
            if (_initOperation != null) throw new Exception($"{Tag}->Initialize. _initOperation != null");
            IsInitialized = true;
            _initOperation = new SharedAsyncOperation().Success();
            return _initOperation;
        }

        public void LaunchTestSuite()
        {
            SharedLogger.Log($"{Tag}->LaunchTestSuite");
        }
    }
}
#endif