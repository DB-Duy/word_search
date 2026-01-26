#if ADJUST
using AdjustSdk;
using Shared.Core.Handler;
using Shared.Core.IoC;
using Shared.Entity.Ump;
using Shared.Service.SharedAdjust;
using Shared.Utils;

namespace Shared.Service.Ump.Handlers
{
    [Component]
    public class AdjustUmpValueSyncHandler : IHandler<UmpEntity>, ISharedUtility, IUmpValueSyncHandler
    {
        public void Handle(UmpEntity e)
        {
            var adjustThirdPartySharing = new AdjustThirdPartySharing(null);
            adjustThirdPartySharing.AddGranularOptionWithLog("google_dma", "eea", $"{e.GdprApplies}");

            if (e.GdprApply)
            {
                var purposeConsentsArr = new int[11];
                var purposeConsent = e.PurposeConsents;

#if UNITY_ANDROID
                var consentLength = purposeConsent.Length;
#elif UNITY_IOS
                var consentLength = (purposeConsent.Length > purposeConsentsArr.Length) ? purposeConsentsArr.Length : purposeConsent.Length; 
#endif
                for (var i = 0; i < consentLength; i++)
                {
                    purposeConsentsArr[i] = purposeConsent[i] - '0'; //convert char to int
                }

                //Denied if Purpose 3 or Purpose 4 has value 0
                var adPersonalizationValue = ((purposeConsentsArr[2] == 0) || (purposeConsentsArr[3] == 0)) ? "0" : "1";
                adjustThirdPartySharing.AddGranularOptionWithLog("google_dma", "ad_personalization", adPersonalizationValue);

                //Denied if Purpose 1 or Purpose 7 has value 0
                var adUserDataValue = (purposeConsentsArr[0] == 0) || (purposeConsentsArr[6] == 0) ? "0" : "1";
                adjustThirdPartySharing.AddGranularOptionWithLog("google_dma", "ad_user_data", adUserDataValue);
            }
            else
            {
                adjustThirdPartySharing.AddGranularOptionWithLog("google_dma", "ad_personalization", "1");
                adjustThirdPartySharing.AddGranularOptionWithLog("google_dma", "ad_user_data", "1");
            }
            this.LogInfo(SharedLogTag.UmpNAdjust_, nameof(adjustThirdPartySharing), adjustThirdPartySharing);

            AdjustSdk.Adjust.TrackThirdPartySharing(adjustThirdPartySharing);
        }
    }
}
#endif