using System.Collections.Generic;
using Newtonsoft.Json;
using Shared.Service.Tracking.Common;
using Shared.Tracking.Templates;

namespace Shared.Service.Tracking.TrackingEvent.Ads
{
    public class AdShowFailedParams : BaseTrackingEvent, IConvertableEvent, ICountEvent
    {
        public override string EventName => "ad_show_failed";
        public string CountKey { get; }

        [JsonProperty("ad_units")] public AdUnit ADUnits { get; }
        [JsonProperty("placement")] public string Placement { get; }
        [JsonProperty("log")] public string Log { get; }
        [JsonProperty("count")] public long Count { get; private set; }
        [JsonProperty("mediation_ad_unit_name")] public string MediationAdUnitName { get; private set; }
        [JsonProperty("mediation_ad_unit_id")] public string MediationAdUnitId { get; private set; }
        
        [JsonIgnore] private Dictionary<string, object> _params;
        
        public static AdShowFailedParams Interstitial(string placement, string errorCode, string des, string mediationAdUnitName, string mediationAdUnitId) => new(AdUnit.Interstitial, TrackingConst.InterstitialAdShowFailedCounterId, placement, errorCode, des, mediationAdUnitName, mediationAdUnitId);
        public static AdShowFailedParams Rewarded(string placement, object errorCode, string des, string mediationAdUnitName, string mediationAdUnitId) => new(AdUnit.Rewarded, TrackingConst.RewardedAdShowFailedCounterId, placement, errorCode.ToString(), des, mediationAdUnitName, mediationAdUnitId);
        public static AdShowFailedParams Audio(string placement, string errorCode, string des) => new(AdUnit.Audio, TrackingConst.AudioAdShowFailedCounterId, placement, errorCode, des, null, null);

        protected AdShowFailedParams(AdUnit adUnits, string counterKey, string placement, string errorCode, string des, string mediationAdUnitName, string mediationAdUnitId)
        {
            ADUnits = adUnits;
            CountKey = counterKey;
            Placement = placement;
            Log = $"{errorCode} - {des}";
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
                { "placement", Placement },
                { "log", Log },
                { "count", Count },
                { "mediation_ad_unit_name", MediationAdUnitName ?? "null" },
                { "mediation_ad_unit_id", MediationAdUnitId ?? "null" },
            };
        }
    }
}