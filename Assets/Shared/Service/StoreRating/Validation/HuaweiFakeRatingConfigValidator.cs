#if HUAWEI
using System.Collections.Generic;
using Newtonsoft.Json;
using Shared.Core.Repository.IntType;
using Shared.Core.Repository.RemoteConfig;
using Shared.Core.Validator;
using Shared.Repository.RemoteConfig;
using Shared.Repository.RemoteConfig.Models;
using Shared.Service.StoreRating.Validation;
using Shared.Utils;

namespace Shared.Rating.Validation
{
    public class HuaweiFakeRatingConfigValidator : IValidator
    {
        // ReSharper disable once InconsistentNaming
        private const string TAG = "HuaweiFakeRatingConfigValidator";
        
        private readonly IRemoteConfigRepository<HuaweiFakeRatingPopup> _remoteConfigRepository;
        private readonly IIntRepository _fakeRatingPopupShownCountLocalRepository;
        private readonly IIntRepository _ratingInvokeCountLocalRepository;
        private readonly IIntRepository _sessionCountRepository;
        private readonly IIntRepository _sessionShowedRatingRepository;
        private int _playedCount;
        
        public HuaweiFakeRatingConfigValidator(IRemoteConfigRepository<HuaweiFakeRatingPopup> remoteConfigRepository, IIntRepository fakeRatingPopupShownCountLocalRepository, IIntRepository ratingInvokeCountLocalRepository, IIntRepository sessionShowedRatingRepository, IIntRepository sessionCountRepository)
        {
            _remoteConfigRepository = remoteConfigRepository;
            _fakeRatingPopupShownCountLocalRepository = fakeRatingPopupShownCountLocalRepository;
            _ratingInvokeCountLocalRepository = ratingInvokeCountLocalRepository;
            _sessionCountRepository = sessionCountRepository;
            _sessionShowedRatingRepository = sessionShowedRatingRepository;
        }

        // {"after_session":10,"show_after_level":1,"interval_session":5,"show_popup":2}
        public bool Validate()
        {
            _playedCount++; 
            var config = _remoteConfigRepository.Get();
            var fakeRatingPopupShownCount = _fakeRatingPopupShownCountLocalRepository.Get();
            var sessionCount = _sessionCountRepository.Get();
            var sessionShowed = _sessionShowedRatingRepository.Get();
#if LOG_INFO
            Dictionary<string, object> logDict = new()
            {
                {"config", config},
                {"fakeRatingPopupShownCount", fakeRatingPopupShownCount},
                {"_playedCount", _playedCount},
                {"sessionCount", sessionCount},
                {"sessionShowed", sessionShowed}
            };
            SharedLogger.Log($"{TAG}->Validate: {JsonConvert.SerializeObject(logDict)}");
#endif
            if (_ratingInvokeCountLocalRepository.Get() > 1) return false;
            if (sessionCount <= config.AfterSession) return false;
            if (_playedCount <= config.NumberOfPlayed) return false;
            if ((sessionCount - sessionShowed) < config.IntervalSession) return false;
            if (fakeRatingPopupShownCount >= config.MaxShownCount) return false;
            _sessionShowedRatingRepository.Set(sessionCount);
            return true;
        }
    }
}
#endif