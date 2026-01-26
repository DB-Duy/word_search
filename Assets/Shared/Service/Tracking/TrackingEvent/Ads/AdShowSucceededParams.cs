using System.Collections.Generic;
using Newtonsoft.Json;
using Shared.Service.Tracking.Common;
using Shared.Tracking.Templates;

namespace Shared.Service.Tracking.TrackingEvent.Ads
{
    public class AdShowSucceededParams : BaseTrackingEvent, IConvertableEvent, ICountEvent
    {
        [JsonProperty("event_name")] public override string EventName => "ad_show_succeeded";
        [JsonProperty("count_key")] public string CountKey { get; }

        [JsonProperty("ad_units")] public AdUnit ADUnits { get; }
        [JsonProperty("placement")] public string Placement { get; }
        [JsonProperty("count")] public long Count { get; private set; }
        [JsonProperty("mediation_ad_unit_name")] public string MediationAdUnitName { get; private set; }
        [JsonProperty("mediation_ad_unit_id")] public string MediationAdUnitId { get; private set; }

        [JsonIgnore] private Dictionary<string, object> _params;
        
        public static AdShowSucceededParams Interstitial(string placement, string mediationAdUnitName, string mediationAdUnitId) => new(AdUnit.Interstitial, TrackingConst.InterstitialAdShowSucceededCounterId, placement, mediationAdUnitName, mediationAdUnitId);
        public static AdShowSucceededParams Rewarded(string placement, string mediationAdUnitName, string mediationAdUnitId) => new(AdUnit.Rewarded, TrackingConst.RewardedAdShowSucceededCounterId, placement, mediationAdUnitName, mediationAdUnitId);
        public static AdShowSucceededParams Audio(string placement) => new(AdUnit.Audio, TrackingConst.AudioAdShowSucceededCounterId, placement, null, null);

        protected AdShowSucceededParams(AdUnit adUnits, string counterKey, string placement, string mediationAdUnitName, string mediationAdUnitId)
        {
            ADUnits = adUnits;
            CountKey = counterKey;
            Placement = placement;
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
                { "count", Count },
                { "mediation_ad_unit_name", MediationAdUnitName ?? "null" },
                { "mediation_ad_unit_id", MediationAdUnitId ?? "null" },
            };
        }
    }
}