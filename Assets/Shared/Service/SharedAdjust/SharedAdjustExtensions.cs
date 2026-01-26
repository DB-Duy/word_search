#if ADJUST
using System.Collections.Generic;
using AdjustSdk;

namespace Shared.Service.SharedAdjust
{
    public static class SharedAdjustExtensions
    {
        private const string Tag = "SharedAdjustExtensions";
        
        public static Dictionary<string, object> ToDict(this AdjustConfig c)
        {
            return new Dictionary<string, object>
            {
                {"AppToken", c.AppToken},
                {"DefaultTracker", c.DefaultTracker},
                {"ExternalDeviceId", c.ExternalDeviceId},
                {"IsCoppaComplianceEnabled", c.IsCoppaComplianceEnabled},
                {"IsSendingInBackgroundEnabled", c.IsSendingInBackgroundEnabled},
                {"IsCostDataInAttributionEnabled", c.IsCostDataInAttributionEnabled},
                {"IsDeviceIdsReadingOnceEnabled", c.IsDeviceIdsReadingOnceEnabled},
                {"IsDeferredDeeplinkOpeningEnabled", c.IsDeferredDeeplinkOpeningEnabled},
                {"AllowSuppressLogLevel", c.AllowSuppressLogLevel},
                {"IsDataResidency", c.IsDataResidency},
                {"ShouldUseSubdomains", c.ShouldUseSubdomains},
                {"EventDeduplicationIdsMaxSize", c.EventDeduplicationIdsMaxSize},
                {"UrlStrategyDomains", c.UrlStrategyDomains},
                {"LogLevel", c.LogLevel},
                {"Environment", c.Environment},
                
                // iOS
                {"IsAdServicesEnabled", c.IsAdServicesEnabled},
                {"IsIdfaReadingEnabled", c.IsIdfaReadingEnabled},
                {"IsSkanAttributionEnabled", c.IsSkanAttributionEnabled},
                {"IsLinkMeEnabled", c.IsLinkMeEnabled},
                {"AttConsentWaitingInterval", c.AttConsentWaitingInterval},
                
                // Android
                {"IsPlayStoreKidsComplianceEnabled", c.IsPlayStoreKidsComplianceEnabled},
                {"IsPreinstallTrackingEnabled", c.IsPreinstallTrackingEnabled},
                {"PreinstallFilePath", c.PreinstallFilePath},
                {"FbAppId", c.FbAppId}
            };
        }
    }
}
#endif