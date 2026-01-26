using Shared.Core.Handler.Corou.Initializable;

namespace Shared.Service.Ads
{
    public interface IBannerAd : IInitializable
    {
        bool Validate();
        
        void LoadBanner();
        void DestroyBanner();
    }
}