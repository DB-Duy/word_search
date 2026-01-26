#if FACEBOOK_INSTANT
using System.Collections.Generic;
using Shared.Ads.Validator;
using Shared.Utils;

namespace Shared.Ads.FacebookInstant.Interstitial.Validation
{
    public class FacebookInstantInterstitialEndLevelSpaceValidator : IAdValidator
    {
        private const string Tag = "FacebookInstantInterstitialEndLevelSpaceValidator";

        private readonly List<string> _placements;
        private int _levelCount = 1000;

        public FacebookInstantInterstitialEndLevelSpaceValidator(params string[] placements)
        {
            _placements = new List<string>(placements);
        }

        public IAdValidationResult Validate(IAdValidationParams entity)
        {
            SharedLogger.Log($"{Tag}->Validate");
            if (entity is not IAdPlacementValidationParams placementParams) return new AdValidationResult(Tag).Ignore("entity is not IAdPlacementValidationParams");
            if (entity is not IFacebookInstantInterstitialValidationParams configParams) return new AdValidationResult(Tag).Ignore("entity is not IFacebookInstantInterstitialValidationParams");
            if (_placements == null || _placements.Count == 0) return new AdValidationResult(Tag).Ignore("_placements == null || _placements.Count == 0");
            if (!_placements.Contains(placementParams.Placement.Name)) return new AdValidationResult(Tag).Ignore("!_placements.Contains(placementParams.Placement.Name)");
            _levelCount++;
            if (_levelCount < configParams.InterstitialConfig.LevelSpace) 
                return new AdValidationResult(Tag).Fail($"_levelCount({_levelCount}) < configParams.InterstitialConfig.LevelSpace({configParams.InterstitialConfig.LevelSpace})");
            _levelCount = 0;
            return new AdValidationResult(Tag).Success();
        }
    }
}
#endif