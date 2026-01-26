using System.Collections.Generic;
using Newtonsoft.Json;
using Shared.Service.Tracking.Common;
using Shared.Tracking;
using Shared.Tracking.Templates;

namespace Shared.Service.Tracking.TrackingEvent.Ads
{
    public class AdClickedParams : BaseTrackingEvent, IConvertableEvent, ICountEvent
    {
        public override string EventName => "ad_clicked";
        [JsonProperty("count_key")] public string CountKey { get; }

        [JsonProperty("ad_units")] public AdUnit AdUnit { get; }
        [JsonProperty("placement")] public string Placement { get; }
        [JsonProperty("count")] public long Count { get; private set; }
        [JsonProperty("mediation_ad_unit_name")] public string MediationAdUnitName { get; private set; }
        [JsonProperty("mediation_ad_unit_id")] public string MediationAdUnitId { get; private set; }
        
        [JsonIgnore] private Dictionary<string, object> _params;
        
        public static AdClickedParams Banner(string placement, string mediationAdUnitName, string mediationAdUnitId) => new(AdUnit.Banner, TrackingConst.BannerAdClickedCounterId, placement, mediationAdUnitName, mediationAdUnitId);
        public static AdClickedParams Interstitial(string placement, string mediationAdUnitName, string mediationAdUnitId) => new(AdUnit.Interstitial, TrackingConst.InterstitialAdClickedCounterId, placement, mediationAdUnitName, mediationAdUnitId);
        public static AdClickedParams Rewarded(string placement, string mediationAdUnitName, string mediationAdUnitId) => new(AdUnit.Rewarded, TrackingConst.RewardedAdClickedCounterId, placement, mediationAdUnitName, mediationAdUnitId);
        public static AdClickedParams Audio(string placement) => new(AdUnit.Audio, TrackingConst.AudioAdClickedCounterId, placement, null, null);
        public static AdClickedParams Inplay(string placement) => new(AdUnit.Inplay, TrackingConst.InplayAdClickedCounterId, placement, null, null);

        private AdClickedParams(AdUnit adUnit, string countKey, string placement, string mediationAdUnitName, string mediationAdUnitId)
        {
            AdUnit = adUnit;
            CountKey = countKey;
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
                { "ad_units", AdUnit.Value },
                { "placement", Placement },
                { "count", Count },
                { "mediation_ad_unit_name", MediationAdUnitName ?? "null" },
                { "mediation_ad_unit_id", MediationAdUnitId ?? "null" },
            };
        }
    }
}