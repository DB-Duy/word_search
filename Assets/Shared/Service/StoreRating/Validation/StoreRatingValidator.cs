#if GOOGLE_PLAY || APPSTORE || UNITY_EDITOR
using Shared.Core.IoC;
using Shared.Core.Validator;
using Shared.Repository.Level;
using Shared.Repository.StoreRating;
using Shared.Utilities;
using Shared.Utils;
using Zenject;

namespace Shared.Service.StoreRating.Validation
{
    [Component]
    public class StoreRatingValidator : IValidator, ISharedUtility, ISharedLogTag, IStoreRatingValidator
    {
        [Inject] private StoreRatingConfigRepository _configRepository;
        [Inject] private StoreRatingShownCountRepository _shownCountRepository;
        [Inject] private LevelRepository _levelRepository;

        public bool Validate()
        {
            var config = _configRepository.Get();
            var savedCount = _shownCountRepository.Get();
            var level = _levelRepository.Get();

            if (savedCount >= config.MaxShownCount)
            {
                this.LogInfo("reason", "Max shown count reached");
                return false;
            }

            if (level < config.AfterLevel)
            {
                this.LogInfo("reason", "Min level to show");
                return false;
            }

            return true;
        }

        public string LogTag => SharedLogTag.StoreRating;
    }
}
#endif