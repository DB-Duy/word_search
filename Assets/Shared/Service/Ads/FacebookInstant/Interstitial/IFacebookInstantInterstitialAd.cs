#if FACEBOOK_INSTANT
using RemoteConfig;
using Shared.PlayerPrefsRepository.RemoteConfig;

namespace Shared.Ads.FacebookInstant.Interstitial
{
    public interface IFacebookInstantInterstitialAd
    {
        IRemoteConfigRepository<InterstitialConfig> ConfigRepository { get; set; }
    }
}
#endif