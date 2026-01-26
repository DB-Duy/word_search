#if (USE_APS || APS) && LEVEL_PLAY
using AmazonAds;
using Shared.Core.Async;
using Shared.Core.IoC;
using Shared.Entity.Ads;
using Shared.Repository.Ump;
using Shared.Service.Aps;
using Shared.Utils;
using Zenject;

namespace Shared.Service.Ads.SharedLevelPlay.Banner
{
    /// <summary>
    /// public static ApsBannerConfig NewPhoneInstance(string id) =&gt; new(320, 50, id);
    /// public static ApsBannerConfig NewTabletInstance(string id) =&gt; new(728, 90, id);
    /// var apsBannerConfig = isTablet ? ApsBannerConfig.NewTabletInstance(config.ApsBanner_728x90) : ApsBannerConfig.NewPhoneInstance(config.ApsBanner_320x50);
    /// </summary>
    [Component]
    public class ApsBannerFetcher : IApsBannerFetcher, ISharedUtility
    {   
        [Inject] private UsPrivacyStringRepository _usPrivacyStringRepository;
        
        public IAsyncOperation Handle(ApsBannerConfig apsSlotId)
        {
            var asyncOperation = new SharedAsyncOperation();

            var adNetworkInfo = new AdNetworkInfo(ApsAdNetwork.UNITY_LEVELPLAY);
            var adRequest = new APSBannerAdRequest(apsSlotId.Width, apsSlotId.Height, apsSlotId.Id, adNetworkInfo);
            var usPrivacyString = _usPrivacyStringRepository?.Get();
            if (!string.IsNullOrEmpty(usPrivacyString))
                adRequest.PutCustomTarget("us_privacy", usPrivacyString);
            adRequest.onFailed += (adError) =>
            {
                SharedLogger.LogInfoCustom(SharedLogTag.AdNLevelPlayNBannerNAps_, nameof(ApsBannerFetcher), "adRequest.onFailed", nameof(adError), adError);
                asyncOperation.Fail(adError);
            };
            adRequest.onFailedWithError += (adError) =>
            {
                SharedLogger.LogInfoCustom(SharedLogTag.AdNLevelPlayNBannerNAps_, nameof(ApsBannerFetcher), "adRequest.onFailedWithError", nameof(adError), adError);
                asyncOperation.Fail($"{adError.GetCode()} - {adError.GetMessage()}");
            };
            adRequest.onSuccess += (adResponse) =>
            {
                var networkData = APSMediationUtils.GetBannerNetworkData(apsSlotId.Id, adResponse);
                SharedLogger.LogInfoCustom(SharedLogTag.AdNLevelPlayNBannerNAps_, nameof(ApsBannerFetcher), "adRequest.onSuccess", nameof(networkData), networkData);
                IronSource.Agent.setNetworkData(APSMediationUtils.APS_IRON_SOURCE_NETWORK_KEY, networkData);
                asyncOperation.Success();
            };
            this.LogInfo(SharedLogTag.AdNLevelPlayNBannerNAps_, nameof(apsSlotId), apsSlotId);
            adRequest.LoadAd();
            return asyncOperation;
        }
    }
}
#endif