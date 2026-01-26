using Shared.Core.Async;
using Shared.Core.Handler.Corou.Initializable;
using Shared.Service.Ads.Common;

namespace Shared.Service.Ads
{
    public interface IRewardAd : IInitializable
    {
        // IResolver<string, RewardedAdShowFailReason> FailMessageResolver { get; set; }
        // IAsyncHandler PartnerLoadHandler { get; set; }
        
        void StartLoadLoop();
        IAsyncOperation<RewardedAdOperation> Show(IAdPlacement placement);
    }
}