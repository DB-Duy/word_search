using System.Collections.Generic;
using Newtonsoft.Json;
using Shared.Service.Tracking.Common;
using Shared.Tracking.Templates;

namespace Shared.Service.Tracking.TrackingEvent.Ads
{
    public class AdLoadSucceededParams : BaseTrackingEvent, IConvertableEvent, ICountEvent
    {
        [JsonProperty("event_name")] public override string EventName => "ad_load_succeeded";
        [JsonProperty("count_key")] public string CountKey { get; }
        
        [JsonProperty("ad_units")] public AdUnit ADUnits { get; }
        [JsonProperty("count")] public long Count { get; private set; }
        [JsonProperty("mediation_ad_unit_name")] public string MediationAdUnitName { get; private set; }
        [JsonProperty("mediation_ad_unit_id")] public string MediationAdUnitId { get; private set; }
        
        [JsonIgnore] private Dictionary<string, object> _params;

        public static AdLoadSucceededParams Banner(string mediationAdUnitName, string mediationAdUnitId) => new(AdUnit.Banner, TrackingConst.BannerAdLoadSucceededCounterId, mediationAdUnitName, mediationAdUnitId);
        public static AdLoadSucceededParams Interstitial(string mediationAdUnitName, string mediationAdUnitId) => new(AdUnit.Interstitial, TrackingConst.InterstitialAdLoadSucceededCounterId, mediationAdUnitName, mediationAdUnitId);
        public static AdLoadSucceededParams Rewarded(string mediationAdUnitName, string mediationAdUnitId) => new(AdUnit.Rewarded, TrackingConst.RewardedAdLoadSucceededCounterId, mediationAdUnitName, mediationAdUnitId);
        public static AdLoadSucceededParams Audio() => new(AdUnit.Audio, TrackingConst.AudioAdLoadSucceededCounterId, null, null);

        protected AdLoadSucceededParams(AdUnit adUnits, string countKey, string mediationAdUnitName, string mediationAdUnitId)
        {
            ADUnits = adUnits;
            CountKey = countKey;
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
                { "mediation_ad_unit_name", MediationAdUnitName ?? "null" },
                { "mediation_ad_unit_id", MediationAdUnitId ?? "null" },
            };
        }
    }
}