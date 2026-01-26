#if TOPON
using Newtonsoft.Json;
using RemoteConfig;
using Shared.Ads.Common;
using Shared.Ads.Validator;

namespace Shared.Ads.SharedTopOn.Interstitial
{
    public class TopOnInterstitialValidationParams : IAdValidationParams, IAdPlacementValidationParams
    {
        [JsonProperty("placement")] public IAdPlacement Placement { get; }
        [JsonProperty("interstitialConfig")] public InterstitialConfig InterstitialConfig { get; }

        public TopOnInterstitialValidationParams(IAdPlacement placement, InterstitialConfig interstitialConfig)
        {
            Placement = placement;
            InterstitialConfig = interstitialConfig;
        }

        public override string ToString() => JsonConvert.SerializeObject(this);
    }
}
#endif