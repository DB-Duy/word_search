#if FACEBOOK_INSTANT
using System;
using Shared.Ads.Validator;
using Shared.Utils;

namespace Shared.Ads.FacebookInstant.Interstitial.Validation
{
    public class FacebookInstantInterstitialIntervalValidator : IAdValidator, IAdCloseEventValidator
    {
        private const string TAG = "FacebookInstantInterstitialIntervalValidator";

        private DateTime _closeDateTime = DateTime.MinValue;

        public IAdValidationResult Validate(IAdValidationParams entity)
        {
            SharedLogger.Log($"{TAG}->Validate");
            if (entity is not IFacebookInstantInterstitialValidationParams configParams) 
                return new AdValidationResult(TAG).Ignore("entity is not IFacebookInstantInterstitialValidationParams");
            
            var interval = (DateTime.Now - _closeDateTime).TotalSeconds;
            var config = configParams.InterstitialConfig.IntervalSeconds;
            return interval >= config
                ? new AdValidationResult(TAG).Success()
                : new AdValidationResult(TAG).Fail($"interval({interval}) >= config({config})");
        }

        public void OnAdCloseEvent()
        {
            _closeDateTime = DateTime.Now;
        }
    }
}
#endif