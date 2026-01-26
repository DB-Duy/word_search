using System.Collections.Generic;
using Newtonsoft.Json;
using Shared.Service.Tracking;
using Shared.Service.Tracking.Common;
using Shared.Tracking.Templates;

namespace Shared.Tracking.Models.Ads
{
    public class AdNotReadyParams : BaseTrackingEvent, IConvertableEvent, ICountEvent
    {
        public override string EventName => "ad_not_ready";
        public string CountKey { get; }

        [JsonProperty("ad_units")] public AdUnit ADUnits { get; }
        [JsonProperty("placement")] public string Placement { get; }
        [JsonProperty("count")] public long Count { get; private set; }
        
        [JsonIgnore] private Dictionary<string, object> _params;

        public static AdNotReadyParams Interstitial(string placement) => new(AdUnit.Interstitial, TrackingConst.InterstitialAdNotReadyCounterId, placement);
        public static AdNotReadyParams Rewarded(string placement) => new(AdUnit.Rewarded, TrackingConst.RewardedAdNotReadyCounterId, placement);
        public static AdNotReadyParams Audio(string placement) => new(AdUnit.Audio, TrackingConst.AudioAdNotReadyCounterId, placement);

        protected AdNotReadyParams(AdUnit adUnits, string counterKey, string placement)
        {
            ADUnits = adUnits;
            CountKey = counterKey;
            Placement = placement;
        }
        
        public void SetEventCount(long count) => Count = count;

        public override string ToString() => JsonConvert.SerializeObject(ToConvertableEvent());

        public Dictionary<string, object> ToConvertableEvent()
        {
            return _params ??= new Dictionary<string, object> (ExParams)
            {
                { "ad_units", ADUnits.Value },
                { "placement", Placement },
                { "count", Count }
            };
        }
    }
}