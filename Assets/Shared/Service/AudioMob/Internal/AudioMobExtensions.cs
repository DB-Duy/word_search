#if AUDIO_MOB

using System.Collections.Generic;
using Audiomob;
using Shared.Service.Tracking.TrackingEvent.Ads;
using Shared.Tracking.Models.Ads;

namespace Shared.Service.AudioMob.Internal
{
    public static class AudioMobExtensions
    {
        public static AdImpressionEvent ToAdImpression(this IAudioAd audioAd)
        {
            return new AdImpressionEvent(
                adPlatform: "audiomob",
                adSource: "audiomob",
                adUnitName: audioAd.AdUnitId,
                adFormat: "audio",
                currency: "USD",
                value: audioAd.Ecpm / 1000.0f); // eCPM is per 1000 impressions, so we divide by 1000 to get the value for a single impression
        }
        
        public static Dictionary<string, object> ToDict(this IAudioAd audioAd)
        {
            if(audioAd == null) return new Dictionary<string, object>();
            //Handle case load audioAd failed -> this return null audioAd
            // var logObject = new Dictionary<string, object>
            // {
            //     { "Id", audioAd.Id },
            //     { "AdFormat", audioAd.AdFormat.ToString() },
            //     { "Duration", audioAd.Duration },
            //     { "EstimatedCPM", audioAd.EstimatedCPM },
            //     { "EstimatedRevenue", audioAd.EstimatedRevenue },
            //     { "ExpiryTime", audioAd.ExpiryTime }
            // };

            var logObject = new Dictionary<string, object>
            {
                { "Id", audioAd.Id },
                { "AdUnitId", audioAd.AdUnitId },
                { "Ecpm", audioAd.Ecpm }
            };
            return logObject;
        }
    }
}
#endif