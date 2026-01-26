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
    public class ApsStaticFetcher : IAsyncHandler<ApsInterstitialConfig>, ISharedUtility
    {   
        [Inject] private UsPrivacyStringRepository _usPrivacyStringRepository;

        public IAsyncOperation Handle(ApsInterstitialConfig config)
        {
            if (string.IsNullOrEmpty(config.StaticSlotId)) return new SharedAsyncOperation().Fail("string.IsNullOrEmpty(config.StaticSlotId)");
            IAsyncOperation operation = new SharedAsyncOperation();
            this.LogInfo(SharedLogTag.AdNLevelPlayNInterstitialNAps, nameof(config.StaticSlotId), config.StaticSlotId);
            
            var adNetworkInfo = new AdNetworkInfo(ApsAdNetwork.UNITY_LEVELPLAY);
            var adRequest = new APSInterstitialAdRequest(config.StaticSlotId, adNetworkInfo);
            var usPrivacyString = _usPrivacyStringRepository?.Get();
            if (!string.IsNullOrEmpty(usPrivacyString))
                adRequest.PutCustomTarget("us_privacy", usPrivacyString);

            adRequest.onSuccess += (adResponse) =>
            {
                this.LogInfo(SharedLogTag.AdNLevelPlayNInterstitialNAps, "f", "adRequest.onSuccess", nameof(adResponse), adResponse);
                var data = APSMediationUtils.GetInterstitialNetworkData(config.StaticSlotId, adResponse);
                IronSource.Agent.setNetworkData(APSMediationUtils.APS_IRON_SOURCE_NETWORK_KEY, data);
                operation.Success();
            };
            adRequest.onFailedWithError += (adError) =>
            {
                this.LogInfo(SharedLogTag.AdNLevelPlayNInterstitialNAps, "f", "adRequest.onFailedWithError", nameof(adError), adError);
                operation.Fail($"{adError.GetCode()} - {adError.GetMessage()}");
            };
            adRequest.onFailed += (adError) =>
            {
                this.LogInfo(SharedLogTag.AdNLevelPlayNInterstitialNAps, "f", "adRequest.onFailed", nameof(adError), adError);
                operation.Fail($"{adError}");
            };
            adRequest.LoadAd();
            return operation;
        }
    }
}

#endif