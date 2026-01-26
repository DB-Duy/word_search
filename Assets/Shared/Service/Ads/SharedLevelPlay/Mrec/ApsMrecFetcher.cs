#if (USE_APS || APS) && LEVEL_PLAY
using AmazonAds;
using Shared.Core.Async;
using Shared.Core.IoC;
using Shared.Repository.Ump;
using Shared.Service.Ads.SharedLevelPlay.Banner;
using Shared.Utils;
using Zenject;

namespace Shared.Service.Ads.SharedLevelPlay.Mrec
{
    /// <summary>
    /// MEDIUM_RECTANGLE	Medium Rectangular (MREC)	300 x 250
    /// </summary>
    [Component]
    public class ApsMrecFetcher : IApsMrecFetcher, ISharedUtility
    {   
        [Inject] private UsPrivacyStringRepository _usPrivacyStringRepository;
        
        public IAsyncOperation Handle(string apsSlotId)
        {
            var asyncOperation = new SharedAsyncOperation();

            var adNetworkInfo = new AdNetworkInfo(ApsAdNetwork.UNITY_LEVELPLAY);
            var adRequest = new APSBannerAdRequest(300, 250, apsSlotId, adNetworkInfo);
            var usPrivacyString = _usPrivacyStringRepository?.Get();
            if (!string.IsNullOrEmpty(usPrivacyString))
                adRequest.PutCustomTarget("us_privacy", usPrivacyString);
            adRequest.onFailed += (adError) =>
            {
                SharedLogger.LogInfoCustom(SharedLogTag.AdNLevelPlayNMrecNAps, nameof(ApsBannerFetcher), "adRequest.onFailed", nameof(adError), adError);
                asyncOperation.Fail(adError);
            };
            adRequest.onFailedWithError += (adError) =>
            {
                SharedLogger.LogInfoCustom(SharedLogTag.AdNLevelPlayNMrecNAps, nameof(ApsBannerFetcher), "adRequest.onFailedWithError", nameof(adError), adError);
                asyncOperation.Fail($"{adError.GetCode()} - {adError.GetMessage()}");
            };
            adRequest.onSuccess += (adResponse) =>
            {
                var networkData = APSMediationUtils.GetBannerNetworkData(apsSlotId, adResponse);
                SharedLogger.LogInfoCustom(SharedLogTag.AdNLevelPlayNMrecNAps, nameof(ApsBannerFetcher), "adRequest.onSuccess", nameof(networkData), networkData);
                IronSource.Agent.setNetworkData(APSMediationUtils.APS_IRON_SOURCE_NETWORK_KEY, networkData);
                asyncOperation.Success();
            };
            this.LogInfo(SharedLogTag.AdNLevelPlayNMrecNAps, nameof(apsSlotId), apsSlotId);
            adRequest.LoadAd();
            return asyncOperation;
        }
    }
}
#endif