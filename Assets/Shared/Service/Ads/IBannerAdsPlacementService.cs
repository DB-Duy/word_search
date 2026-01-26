using Shared.View.Ad;

namespace Shared.Service.Ads
{
    public interface IBannerAdsPlacementService
    {
        void Register(BannerAdsPlacement p);
        void Remove(BannerAdsPlacement p);
        bool IsVisible();
        void ValidateAndNotifyChanges();
    }
}