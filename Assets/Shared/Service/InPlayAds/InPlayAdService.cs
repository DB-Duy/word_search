#if INPLAY_ADS
using Shared.Core.Async;
using Shared.Core.IoC;
using Shared.Core.Validator;
using Shared.Service.InPlayAds.Validation;
using Shared.Utils;

namespace Shared.Service.InPlayAds
{
    [Service]
    public class InPlayAdService : IInPlayAdService, ISharedUtility
    {
        private IValidator<string> _validator;
        private IValidator<string> Validator => _validator ??= ValidatorChain<string>.CreateChainFromType<IInPlayAdValidator>();

        
        /// <summary>
        ///  Dùng để đảm bảo khi tới service này được init thì mới cho Mediator chạy.
        /// </summary>
        public bool IsInitialized { get; private set; } = false;
        public IAsyncOperation Initialize()
        {
            IsInitialized = true;
            return new SharedAsyncOperation().Success();
        }
        // -------------------------------------------------------------------------------------------------------------

        public bool ValidateByValidators(string placement) => Validator?.Validate(placement) ?? false;

        public void RemoveAds()
        {
            this.LogInfo(SharedLogTag.InPlayAds);
            foreach (var placement in InPlayAdRegistry.Placements)
                placement.RemoveAds();

            foreach (var mediator in InPlayAdRegistry.Mediators)
                mediator.RemoveAd();
        }
    }
}
#endif