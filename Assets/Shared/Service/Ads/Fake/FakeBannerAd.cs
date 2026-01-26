#if UNITY_EDITOR
using Shared.Core.Async;
using Shared.Core.Handler.Async;
using Shared.Core.IoC;
using Shared.Core.Validator;
using Shared.Utils;

namespace Shared.Service.Ads.Fake
{
    [Service]
    public class FakeBannerAd: IBannerAd
    {
        private const string Tag = "FakeBannerAd";
        
        public IAsyncHandler PreLoadHandler { get; set; }
        public IAsyncHandler OnImpressionEventHandler { get; set; }
        public IValidator Validator { get; set; }

        public bool Validate() => Validator == null || Validator.Validate();

        public void LoadBanner()
        {
            SharedLogger.Log($"{Tag}->LoadBanner");
        }

        public void DestroyBanner()
        {
            SharedLogger.Log($"{Tag}->DestroyBanner");
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