#if FIREBASE
using System.Collections.Generic;
using Firebase.Analytics;
using Shared.Core.Handler;
using Shared.Core.IoC;
using Shared.Entity.Ump;
using Shared.Utils;

namespace Shared.Service.Ump.Handlers
{
    [Component]
    public class FirebaseUmpValueSyncHandler: IHandler<UmpEntity>, ISharedUtility, IUmpValueSyncHandler
    {   
        public void Handle(UmpEntity e)
        {
            if (!e.GdprApply)
            {
                this.LogInfo(SharedLogTag.UmpNFirebase, nameof(e.GdprApply), e.GdprApply);
                return;
            }

            var purposeConsentsArr = new int[11];   
            var purposeConsent = e.PurposeConsents; 
            
#if UNITY_ANDROID
            var consentLength = purposeConsent.Length;
#elif UNITY_IOS
            var consentLength = (purposeConsent.Length > purposeConsentsArr.Length) ? purposeConsentsArr.Length : purposeConsent.Length; 
#endif
            for (var i = 0; i < consentLength; i++)
                purposeConsentsArr[i] = purposeConsent[i] - '0'; //convert char to int
            
            var adStorage = purposeConsentsArr[0] == 0 ? ConsentStatus.Denied : ConsentStatus.Granted;
            // Denied if Purpose 1 has value 0
            var analyticsStorage = purposeConsentsArr[0] == 0 ? ConsentStatus.Denied : ConsentStatus.Granted;
            // Denied if Purpose 1 or Purpose 7 has value 0
            var adUserData = ((purposeConsentsArr[0] == 0) || purposeConsentsArr[6] == 0) ? ConsentStatus.Denied : ConsentStatus.Granted;
            // Denied if Purpose 3 or Purpose 4 has value 0
            var adPersonalization = ((purposeConsentsArr[2] == 0) || purposeConsentsArr[3] == 0) ? ConsentStatus.Denied : ConsentStatus.Granted;
            
            var consentSettings = new Dictionary<ConsentType, ConsentStatus>
            {
                { ConsentType.AdStorage, adStorage },
                { ConsentType.AnalyticsStorage, analyticsStorage },
                { ConsentType.AdUserData, adUserData },
                { ConsentType.AdPersonalization, adPersonalization }
            };
            FirebaseAnalytics.SetConsent(consentSettings);
            this.LogInfo(SharedLogTag.UmpNFirebase, nameof(consentSettings), consentSettings);
        }
    }
}
#endif