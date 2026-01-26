using System.Collections.Generic;
using Newtonsoft.Json;
using Shared.Service.Tracking.Common;
using Shared.Tracking.Templates;

namespace Shared.Service.Tracking.TrackingEvent.Ads
{
    public class AdLoadParams : BaseTrackingEvent, IConvertableEvent, ICountEvent
    {
        [JsonProperty("eventName")] public override string EventName => "ad_load";
        [JsonProperty("countKey")] public string CountKey { get; }
        
        [JsonProperty("ad_units")] public AdUnit ADUnits { get; }
        [JsonProperty("count")] public long Count { get; private set; }
        
        [JsonIgnore] private Dictionary<string, object> _params;

        public static AdLoadParams MrecAdLoadParams() => new(AdUnit.Mrec, TrackingConst.MrecAdLoadCounterId);
        public static AdLoadParams BannerAdLoadParams() => new(AdUnit.Banner, TrackingConst.BannerAdLoadCounterId);
        public static AdLoadParams InterstitialAdLoadParams() => new(AdUnit.Interstitial, TrackingConst.InterstitialAdLoadCounterId);
        public static AdLoadParams RewardedAdLoadParams() => new(AdUnit.Rewarded, TrackingConst.RewardedAdLoadCounterId);
        public static AdLoadParams AudioAdLoadParams() => new(AdUnit.Audio, TrackingConst.AudioAdLoadCounterId);
        
        protected AdLoadParams(AdUnit adUnits, string countKey)
        {
            ADUnits = adUnits;
            CountKey = countKey;
        }
        
        public void SetEventCount(long count) => Count = count;

        public override string ToString() => JsonConvert.SerializeObject(ToConvertableEvent());
        
        public Dictionary<string, object> ToConvertableEvent()
        {
            return _params ??= new Dictionary<string, object> (ExParams)
            {
                { "ad_units", ADUnits.Value },
                { "count", Count }
            };
        }
    }
}