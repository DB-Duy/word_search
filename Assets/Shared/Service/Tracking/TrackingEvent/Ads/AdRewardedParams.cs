using System.Collections.Generic;
using Newtonsoft.Json;
using Shared.Service.Tracking;
using Shared.Service.Tracking.Common;
using Shared.Tracking.Templates;

namespace Shared.Tracking.Models.Ads
{
    public class AdRewardedParams : BaseTrackingEvent, IConvertableEvent, ICountEvent
    {
        [JsonProperty("event_name")] public override string EventName => "ad_rewarded";
        [JsonProperty("count_key")] public string CountKey { get; }

        [JsonProperty("ad_units")] public AdUnit ADUnits { get; }
        [JsonProperty("placement")] public string Placement { get; }
        [JsonProperty("count")] public long Count { get; private set; }
        
        [JsonIgnore] private Dictionary<string, object> _params;
        
        public static AdRewardedParams Rewarded(string placement) => new(AdUnit.Rewarded, TrackingConst.RewardedAdRewardedCounterId, placement);
        
        public AdRewardedParams(AdUnit adUnits, string counterKey, string placement)
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