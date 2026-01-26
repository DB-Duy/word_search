#if UNITY_EDITOR
using Shared.Core.Async;
using Shared.Core.Handler.Async;
using Shared.Core.IoC;
using Shared.Core.Validator;
using Shared.Service.Ads.Common;

namespace Shared.Service.Ads.Fake
{
    [Service]
    public class FakeInterstitialAd : IInterstitialAd
    {
        public IValidator LoadValidator { get; set; }
        public IValidator<string> PlacementValidator { get; set; }
        public IAsyncHandler PreloadHandler { get; set; }
        
        public void StartLoadLoop()
        {
            
        }

        public IAsyncOperation Show(IAdPlacement adPlacement)
        {
            return new SharedAsyncOperation().Success();
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