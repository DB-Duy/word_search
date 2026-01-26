#if GOOGLE_PLAY && USER_RATING
using System.Collections;
using Google.Play.Review;
using Shared.Core.Async;
using Shared.Core.IoC;
using Shared.Core.Validator;
using Shared.Repository.StoreRating;
using Shared.Service.SharedCoroutine;
using Shared.Service.StoreRating.Internal;
using Shared.Service.StoreRating.Validation;
using Shared.Utilities;
using Shared.Utils;
using Zenject;

namespace Shared.Service.StoreRating
{
    [Service]
    public class GooglePlayStoreRatingService : IStoreRatingService, ISharedUtility, ISharedLogTag
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
            _ratingOperation = new SharedAsyncOperation();
            this.StartSharedCoroutine(_Rate());
            return _ratingOperation;
        }

        private IEnumerator _Rate()
        {
            var reviewManager = new ReviewManager();
            var requestFlowOperation = reviewManager.RequestReviewFlow();
            yield return requestFlowOperation;
            this.LogInfo(nameof(requestFlowOperation), requestFlowOperation.ToDict());
            
            if (requestFlowOperation.Error != ReviewErrorCode.NoError)
            {
                this.LogError(nameof(requestFlowOperation.Error), requestFlowOperation.Error.ToString());
                _ratingOperation.Fail($"requestFlowOperation.Error = {requestFlowOperation.Error}");
                _ratingOperation = null;
                yield break;
            }

            var playReviewInfo = requestFlowOperation.GetResult();
            var launchFlowOperation = reviewManager.LaunchReviewFlow(playReviewInfo);
            yield return launchFlowOperation;
            this.LogInfo(nameof(launchFlowOperation), launchFlowOperation.ToDict());
            
            if (launchFlowOperation.Error != ReviewErrorCode.NoError)
            {
                _ratingOperation.Fail($"launchFlowOperation.Error = {launchFlowOperation.Error}");
                _ratingOperation = null;
                yield break;
            }

            _ratingOperation.Success();
            _ratingOperation = null;
        }

        public string LogTag => SharedLogTag.StoreRating;
    }
}
#endif