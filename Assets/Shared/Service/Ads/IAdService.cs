using Shared.Core.Handler.Corou.Initializable;

namespace Shared.Service.Ads
{
    /// <summary>
    /// Singleton, Manage all game ads.
    /// </summary>
    public interface IAdService : IInitializable
    {   
        IBannerAd BannerAd { get; }
        IInterstitialAd InterstitialAd { get; }
        IRewardAd RewardAd { get; }
        
        void LaunchTestSuite();
    }
}