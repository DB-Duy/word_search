#if (APS || USE_APS) && LEVEL_PLAY
using AmazonAds;
using Shared.Core.Async;
using Shared.Core.Handler.Async;
using Shared.Core.IoC;
using Shared.Repository.Ump;
using Shared.Service.Aps;
using Shared.Utils;
using Zenject;

namespace Shared.Service.Ads.SharedLevelPlay.RV
{
    [Component]
    public class ApsRewardFetcher : IAsyncHandler<string>, IApsRewardFetcher
    {
        private const string Tag = "ApsRewardFetcher";
        [Inject] private UsPrivacyStringRepository _usPrivacyStringRepository;

        public IAsyncOperation Handle(string apsSlotId)
        {
            SharedLogger.LogJson(SharedLogTag.AdNLevelPlayNRewardNAps, $"{Tag}->Handle", nameof(apsSlotId), apsSlotId);
            var operation = new SharedAsyncOperation();
            var adNetworkInfo = new AdNetworkInfo(ApsAdNetwork.UNITY_LEVELPLAY);
            var adRequest = new APSVideoAdRequest(320, 480, apsSlotId, adNetworkInfo);
            var usPrivacyString = _usPrivacyStringRepository?.Get();
            if (!string.IsNullOrEmpty(usPrivacyString))
                adRequest.PutCustomTarget("us_privacy", usPrivacyString);
            adRequest.onSuccess += (adResponse) =>
            {
                SharedLogger.LogJson(SharedLogTag.AdNLevelPlayNRewardNAps, $"{Tag}->Handle->_adRequest.onSuccess", nameof(adResponse), adResponse.ToInfoString());
                var data = APSMediationUtils.GetRewardedNetworkData(apsSlotId, adResponse);
                IronSource.Agent.setNetworkData(APSMediationUtils.APS_IRON_SOURCE_NETWORK_KEY, data);
                operation.Success();
            };
            adRequest.onFailedWithError += (adError) =>
            {
                SharedLogger.LogJson(SharedLogTag.AdNLevelPlayNRewardNAps, $"{Tag}->Handle->_adRequest.onFailedWithError", nameof(adError), adError);
                operation.Fail($"{adError.GetCode()} - {adError.GetMessage()}");

            };
            adRequest.LoadAd();
            return operation;
        }
    }
}
#endif