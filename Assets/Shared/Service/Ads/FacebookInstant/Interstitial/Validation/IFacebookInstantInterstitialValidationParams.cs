#if FACEBOOK_INSTANT

using RemoteConfig;

namespace Shared.Ads.FacebookInstant.Interstitial.Validation
{
    public interface IFacebookInstantInterstitialValidationParams
    {
        InterstitialConfig InterstitialConfig { get; }
    }
}
#endif