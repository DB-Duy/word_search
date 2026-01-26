#if (USE_APS || APS) && LEVEL_PLAY
using AmazonAds;
using Shared.Core.Async;
using Shared.Core.Handler.Async;
using Shared.Core.IoC;
using Shared.Entity.Ads;
using Shared.Repository.Ump;
using Shared.Utils;
using Zenject;

namespace Shared.Service.Ads.SharedLevelPlay.FS
{
    [Component]
    public class ApsVideoFetcher : IAsyncHandler<ApsInterstitialConfig>, ISharedUtility
    {
        [Inject] private UsPrivacyStringRepository _usPrivacyStringRepository;
        
        public IAsyncOperation Handle(ApsInterstitialConfig config)
        {
            if (string.IsNullOrEmpty(config.VideoSlotId)) return new SharedAsyncOperation().Fail("string.IsNullOrEmpty(config.VideoSlotId)");
            IAsyncOperation operation = new SharedAsyncOperation();
            this.LogInfo(SharedLogTag.AdNLevelPlayNInterstitialNAps, nameof(config.VideoSlotId), config.VideoSlotId);
            var adNetworkInfo = new AdNetworkInfo(ApsAdNetwork.UNITY_LEVELPLAY);
            var adRequest = new APSVideoAdRequest(320, 480, config.VideoSlotId, adNetworkInfo);
            var usPrivacyString = _usPrivacyStringRepository?.Get();
            if (!string.IsNullOrEmpty(usPrivacyString))
                adRequest.PutCustomTarget("us_privacy", usPrivacyString);
            
            adRequest.onSuccess += (adResponse) =>
            {
                this.LogInfo(SharedLogTag.AdNLevelPlayNInterstitialNAps, "f", "adRequest.onSuccess", nameof(adResponse), adResponse);
                var data = APSMediationUtils.GetInterstitialNetworkData(config.VideoSlotId, adResponse);
                IronSource.Agent.setNetworkData(APSMediationUtils.APS_IRON_SOURCE_NETWORK_KEY, data);
                operation.Success();
            };
            adRequest.onFailedWithError += (adError) =>
            {
                this.LogInfo(SharedLogTag.AdNLevelPlayNInterstitialNAps, "f", "adRequest.onFailedWithError", nameof(adError), adError);
                operation.Fail("");
            };
            adRequest.LoadAd();
            return operation;
        }
    }
}
#endif