#if HUAWEI
using System.Collections.Generic;
using System.Linq;
using HuaweiMobileServices.InAppComment;
using Shared.Common;
using Shared.Rating.Validation;
using Shared.Tracking.Models.Common;
using Shared.Utils;

namespace Shared.Rating
{
    public class HuaweiRatingController : IStoreRatingService, IScreenTrackingHandler
    {
        private const string TAG = "HuaweiRatingController";
        public bool IsInitialized { get; private set; } = false;
        
        private readonly HashSet<IValidator> _validators = new();
        private IAsyncOperation _ratingOperation;
        private readonly IIntRepository _ratingInvokeCountLocalRepository;
        private readonly IIntRepository _fakeRatingPopupShownCountLocalRepository;

        public HuaweiRatingController(IIntRepository ratingInvokeCountLocalRepository, IIntRepository fakeRatingPopupShownCountLocalRepository)
        {
            _ratingInvokeCountLocalRepository = ratingInvokeCountLocalRepository;
            _fakeRatingPopupShownCountLocalRepository = fakeRatingPopupShownCountLocalRepository;
        }

        public IAsyncOperation Initialize()
        {
            if (IsInitialized) new SharedAsyncOperation().Success();
            IsInitialized = true;
            SharedCoreEvents.Tracking.Screen.Add(this);
            return new SharedAsyncOperation().Success();
        }

        public IStoreRatingService AddValidators(params IValidator[] validators)
        {
            _validators.AddRange(validators);
            return this;
        }
        
        public bool Validate()
        {
            return _validators.All(validator => validator.Validate());
        }

        public IAsyncOperation Rate()
        {
            if (_ratingOperation != null) return _ratingOperation;
            _ratingOperation = new SharedAsyncOperation().Success();
            InAppComment.ShowInAppComment(_CheckResultMeans);
            _ratingInvokeCountLocalRepository?.AddMore(1);
            return _ratingOperation;
        }
        
        private void _CheckResultMeans(int result)
        {
            switch (result)
            {
                case 101:
                    SharedLogger.Log($"{TAG} CheckResultMeans ResultCode:{result}: : The app has not been released on AppGallery.");
                    break;
                case 102:
                    SharedLogger.Log($"{TAG} CheckResultMeans ResultCode:{result}: : Rating submitted.");
                    break;
                case 103:
                    SharedLogger.Log($"{TAG} CheckResultMeans ResultCode:{result}: : Comment submitted.");
                    break;
                case 104:
                    SharedLogger.Log($"{TAG} CheckResultMeans ResultCode:{result}: : The HUAWEI ID sign-in status is invalid.");
                    break;
                case 105:
                    SharedLogger.Log($"{TAG} CheckResultMeans ResultCode:{result}: : The user does not meet the conditions for displaying the comment pop-up.");
                    break;
                case 106:
                    SharedLogger.Log($"{TAG} CheckResultMeans ResultCode:{result}: : The commenting function is disabled.");
                    break;
                case 107:
                    SharedLogger.Log($"{TAG} CheckResultMeans ResultCode:{result}: : The in-app commenting service is not supported. (Apps released in the Chinese mainland do not support this service.)");
                    break;
                case 108:
                    SharedLogger.Log($"{TAG} CheckResultMeans ResultCode:{result}: : The user canceled the comment.)");
                    break;
                default:
                    SharedLogger.Log($"{TAG} CheckResultMeans Default ResultCode:{result}");
                    break;
            }
        }

        public void OnScreenTrackingEvent(string newScreenName)
        {
            if (TrackingScreen.FakeRating.Value != newScreenName)
            {
                SharedLogger.Log($"{TAG}->OnScreenTrackingEvent: IGNORE: TrackingScreen.FakeRating.Value != newScreenName({newScreenName})");
                return;
            }

            var count = _fakeRatingPopupShownCountLocalRepository?.AddMore(1);
            SharedLogger.Log($"{TAG}->OnScreenTrackingEvent: _fakeRatingPopupShownCountLocalRepository={count}");
        }        
    }
}
#endif