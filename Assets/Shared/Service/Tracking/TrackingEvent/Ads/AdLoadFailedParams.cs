using System.Collections.Generic;
using Newtonsoft.Json;
using Shared.Service.Tracking.Common;
using Shared.Tracking.Templates;

namespace Shared.Service.Tracking.TrackingEvent.Ads
{
    public class AdLoadFailedParams : BaseTrackingEvent, IConvertableEvent, ICountEvent
    {
        public override string EventName => "ad_load_failed";
        public string CountKey { get; }

        [JsonProperty("ad_units")] public AdUnit ADUnits { get; }
        [JsonProperty("count")] public long Count { get; private set; }
        [JsonProperty("log")] public string Log { get; }
        [JsonProperty("mediation_ad_unit_name")] public string MediationAdUnitName { get; private set; }
        [JsonProperty("mediation_ad_unit_id")] public string MediationAdUnitId { get; private set; }
        
        [JsonIgnore] private Dictionary<string, object> _params;
       
        public static AdLoadFailedParams Banner(string errorCode, string des, string mediationAdUnitName, string mediationAdUnitId) => new(AdUnit.Banner, TrackingConst.BannerAdLoadFailedCounterId, errorCode, des, mediationAdUnitName, mediationAdUnitId);
        public static AdLoadFailedParams Banner(string log, string mediationAdUnitName, string mediationAdUnitId) => new(AdUnit.Banner, TrackingConst.BannerAdLoadFailedCounterId, log, mediationAdUnitName, mediationAdUnitId);
        public static AdLoadFailedParams Interstitial(string errorCode, string des, string mediationAdUnitName, string mediationAdUnitId) => new(AdUnit.Interstitial, TrackingConst.InterstitialAdLoadFailedCounterId, errorCode, des, mediationAdUnitName, mediationAdUnitId);
        public static AdLoadFailedParams Rewarded(string errorCode, string des, string mediationAdUnitName, string mediationAdUnitId) => new(AdUnit.Rewarded, TrackingConst.RewardedAdLoadFailedCounterId, errorCode, des, mediationAdUnitName, mediationAdUnitId);
        public static AdLoadFailedParams Audio(string errorCode, string des) => new(AdUnit.Audio, TrackingConst.AudioAdLoadFailedCounterId, errorCode, des, null, null);

        protected AdLoadFailedParams(AdUnit adUnits, string countKey, string errorCode, string des, string mediationAdUnitName, string mediationAdUnitId)
        {
            ADUnits = adUnits;
            CountKey = countKey;
            Log = $"{errorCode} - {des}";
            MediationAdUnitName = mediationAdUnitName;
            MediationAdUnitId = mediationAdUnitId;
        }

        protected AdLoadFailedParams(AdUnit adUnits, string countKey, string log, string mediationAdUnitName, string mediationAdUnitId)
        {
            ADUnits = adUnits;
            CountKey = countKey;
            Log = log;
            MediationAdUnitName = mediationAdUnitName;
            MediationAdUnitId = mediationAdUnitId;
        }

        public void SetEventCount(long count) => Count = count;
        public override string ToString() => JsonConvert.SerializeObject(ToConvertableEvent());

        public Dictionary<string, object> ToConvertableEvent()
        {
            return _params ??= new Dictionary<string, object> (ExParams)
            {
                { "ad_units", ADUnits.Value },
                { "count", Count },
                { "log", Log },
                { "mediation_ad_unit_name", MediationAdUnitName ?? "null" },
                { "mediation_ad_unit_id", MediationAdUnitId ?? "null" },
            };
        }
    }
}