#if FACEBOOK_INSTANT
using Newtonsoft.Json;
using RemoteConfig;
using Shared.Ads.Common;
using Shared.Ads.Validator;

namespace Shared.Ads.FacebookInstant.Interstitial.Validation
{
    public class FacebookInstantInterstitialValidationParams : IAdValidationParams, IAdPlacementValidationParams, IFacebookInstantInterstitialValidationParams
    {
        [JsonProperty("placement")] public IAdPlacement Placement { get; }
        [JsonProperty("interstitialConfig")] public InterstitialConfig InterstitialConfig { get; }

        public FacebookInstantInterstitialValidationParams(IAdPlacement placement, InterstitialConfig interstitialConfig)
        {
            Placement = placement;
            InterstitialConfig = interstitialConfig;
        }

        public override string ToString() => JsonConvert.SerializeObject(this);
    }
}
#endif