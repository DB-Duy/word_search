#if APPSTORE || APPSTORE_CHINA

using Shared.Core.Async;
using Shared.Core.IoC;
using Shared.Core.Validator;
using Shared.Repository.StoreRating;
using Shared.Service.StoreRating.Validation;
using UnityEngine;
using Zenject;

namespace Shared.Service.StoreRating
{
    [Service]
    public class AppStoreRatingService : IStoreRatingService
    {
        [Inject] private StoreRatingShownCountRepository _shownCountRepository;
        
        private IValidator _validator;
        private IValidator Validator => _validator ??= ValidatorChain.CreateChainFromType<IStoreRatingValidator>();
        
        private IAsyncOperation _ratingOperation;
        
        public bool Validate() => Validator?.Validate() ?? true;
        
        public void IncreaseShownCountByOne() => _shownCountRepository.AddMore(1);

        public IAsyncOperation Rate()
        {
            if (_ratingOperation != null) return _ratingOperation;
            _ratingOperation = new SharedAsyncOperation().Success();
            UnityEngine.iOS.Device.RequestStoreReview();
            return _ratingOperation;
        }
        
    }
}
#endif