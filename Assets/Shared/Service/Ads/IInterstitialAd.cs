using Shared.Core.Async;
using Shared.Core.Handler.Corou.Initializable;
using Shared.Service.Ads.Common;

namespace Shared.Service.Ads
{
    public interface IInterstitialAd : IInitializable
    {
        void StartLoadLoop();
        IAsyncOperation Show(IAdPlacement adPlacement);
    }
}