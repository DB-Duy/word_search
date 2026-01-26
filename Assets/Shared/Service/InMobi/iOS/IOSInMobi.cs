#if INMOBI && UNITY_IOS
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Facebook.Unity;
using Shared.Utils;

namespace Shared.Service.InMobi
{
    /// <summary>
    /// https://support.inmobi.com/monetize/sdk-documentation/ios-guidelines/overview-ios-guidelines
    /// </summary>
    public class IOSInMobi : ISharedInMobi, ISharedUtility
    {
        [DllImport("__Internal")] private static extern void _InMobiUpdateGDPRConsent(string jsonConsent);
        [DllImport("__Internal")] private static extern void _InMobiSetUSPrivacyString(string usPrivacyString);
        
        private const string Tag = "IOSInMobi";
        
        public void UpdateGdprConsent(Dictionary<string, object> consentObject)
        {
            this.LogInfo(SharedLogTag.UmpNInMobi, $"{Tag}->UpdateGdprConsent", nameof(consentObject), consentObject);
            if (consentObject == null || consentObject.Count == 0) return;
            var json = consentObject.ToJson();
            _InMobiUpdateGDPRConsent(json);
        }
        
        public void SetUSPrivacyString(string usPrivacyString)
        {
            this.LogInfo(SharedLogTag.UmpNInMobi, $"{Tag}->SetUSPrivacyString", nameof(usPrivacyString), usPrivacyString);
            if (string.IsNullOrEmpty(usPrivacyString)) return;
            _InMobiSetUSPrivacyString(usPrivacyString);
        }
    }
} 
#endif