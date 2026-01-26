#if FACEBOOK_INSTANT
namespace Shared.Ads.FacebookInstant.Config
{
    public interface IFacebookInstantAdConfig
    {
        string FacebookId { get; }
        string FacebookRewardedId { get; }
        string FacebookInterstitialId { get; }
        string FacebookBannerId { get; }
    }
}
#endif