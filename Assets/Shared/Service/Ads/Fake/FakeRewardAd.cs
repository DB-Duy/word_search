#if UNITY_EDITOR
using Shared.Core.Async;
using Shared.Core.Handler.Async;
using Shared.Core.IoC;
using Shared.Core.Resolver;
using Shared.Service.Ads.Common;

namespace Shared.Service.Ads.Fake
{
    [Service]
    public class FakeRewardAd : IRewardAd
    {
        public IResolver<string, RewardedAdShowFailReason> FailMessageResolver { get; set; }
        public IAsyncHandler PartnerLoadHandler { get; set; }
        

        public void StartLoadLoop()
        {
        }

        public IAsyncOperation<RewardedAdOperation> Show(IAdPlacement placement)
        {
            return new SharedAsyncOperation<RewardedAdOperation>(new RewardedAdOperation(placement.Name)).Success();
        }
        
        public bool IsInitialized { get; private set; }
        private IAsyncOperation _initOperation;
        public IAsyncOperation Initialize()
        {
            if (_initOperation != null) return _initOperation;
            IsInitialized = true;
            _initOperation = new SharedAsyncOperation().Success();
            return _initOperation;
        }
    }
}
#endif