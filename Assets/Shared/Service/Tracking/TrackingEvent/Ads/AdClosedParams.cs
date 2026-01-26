using System.Collections.Generic;
using Newtonsoft.Json;
using Shared.Service.Tracking.Common;
using Shared.Tracking.Templates;

namespace Shared.Service.Tracking.TrackingEvent.Ads
{
    public class AdClosedParams : BaseTrackingEvent, IConvertableEvent, ICountEvent
    {
        public override string EventName => "ad_closed";
        public string CountKey { get; }

        [JsonProperty("ad_units")] public AdUnit ADUnits { get; }
        [JsonProperty("placement")] public string Placement { get; }
        [JsonProperty("count")] public long Count { get; private set; }
        [JsonProperty("mediation_ad_unit_name")] public string MediationAdUnitName { get; private set; }
        [JsonProperty("mediation_ad_unit_id")] public string MediationAdUnitId { get; private set; }
        
        [JsonIgnore] private Dictionary<string, object> _params;

        public static AdClosedParams Interstitial(string placement, string mediationAdUnitName, string mediationAdUnitId) => new(AdUnit.Interstitial, TrackingConst.InterstitialAdClosedCounterId, placement, mediationAdUnitName, mediationAdUnitId);
        public static AdClosedParams Rewarded(string placement, string mediationAdUnitName, string mediationAdUnitId) => new(AdUnit.Rewarded, TrackingConst.RewardedAdClosedCounterId, placement, mediationAdUnitName, mediationAdUnitId);
        public static AdClosedParams Audio(string placement) => new(AdUnit.Audio, TrackingConst.AudioAdClosedCounterId, placement, null, null);

        protected AdClosedParams(AdUnit adUnits, string countKey, string placement, string mediationAdUnitName, string mediationAdUnitId)
        {
            ADUnits = adUnits;
            CountKey = countKey;
            Placement = placement;
            MediationAdUnitName = mediationAdUnitName;
            MediationAdUnitId = mediationAdUnitId;
        }
        public override string ToString() => JsonConvert.SerializeObject(ToConvertableEvent());
        
        public void SetEventCount(long count) => Count = count;

        public Dictionary<string, object> ToConvertableEvent()
        {
            return _params ??= new Dictionary<string, object> (ExParams)
            {
                { "ad_units", ADUnits.Value },
                { "placement", Placement },
                { "count", Count },
                { "mediation_ad_unit_name", MediationAdUnitName ?? "null" },
                { "mediation_ad_unit_id", MediationAdUnitId ?? "null" },
            };
        }
    }
}