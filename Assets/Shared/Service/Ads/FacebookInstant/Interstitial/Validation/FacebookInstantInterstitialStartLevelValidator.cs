#if FACEBOOK_INSTANT
using Shared.Ads.Validator;
using Shared.Utils;

namespace Shared.Ads.FacebookInstant.Interstitial.Validation
{
    public class FacebookInstantInterstitialStartLevelValidator : IAdValidator
    {
        private const string TAG = "FacebookInstantInterstitialStartLevelValidator";
        private readonly IIntRepository _levelRepository;

        public FacebookInstantInterstitialStartLevelValidator(IIntRepository levelRepository)
        {
            _levelRepository = levelRepository;
        }

        public IAdValidationResult Validate(IAdValidationParams entity)
        {
            SharedLogger.Log($"{TAG}->Validate");
            if (entity is not IFacebookInstantInterstitialValidationParams configParams) 
                return new AdValidationResult(TAG).Ignore("entity is not IFacebookInstantInterstitialValidationParams");
            var currentLevel = _levelRepository.Get();
            var config = configParams.InterstitialConfig;

            return currentLevel >= config.EnableAfterLevel
                ? new AdValidationResult(TAG).Success()
                : new AdValidationResult(TAG).Fail($"currentLevel({currentLevel}) >= config.EnableAfterLevel({config.EnableAfterLevel})");
        }
    }
}
#endif