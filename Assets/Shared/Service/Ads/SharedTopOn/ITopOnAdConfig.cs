#if TOPON
namespace Shared.Ads.SharedTopOn
{
    public interface ITopOnAdConfig : IAdConfig
    {
        string TopOnAppId { get; }
        string TopOnAppKey { get; }
        string TopOnRewarded { get; }
        string TopOnInterstitial { get; }
        string TopOnBanner { get; }
    }
}
#endif