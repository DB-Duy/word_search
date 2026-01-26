#if UNITY_ANDROID && INMOBI 

using System.Collections.Generic;
using Shared.Utils;
using UnityEngine;

namespace Shared.Service.InMobi
{
    /// <summary>
    /// https://support.inmobi.com/monetize/sdk-documentation/android-guidelines/overview-android-guidelines
    /// </summary>
    public class AndroidInMobi : ISharedInMobi, ISharedUtility
    {
        private const string Tag = "AndroidInMobi";

        private static readonly AndroidJavaClass _inMobiSdk = new("com.inmobi.sdk.InMobiSdk");
        
        private static readonly AndroidJavaClass _inMobiPrivacyCompliance = new("com.inmobi.compliance.InMobiPrivacyCompliance");

        public void UpdateGdprConsent(Dictionary<string, object> consentObject)
        {
            this.LogInfo(SharedLogTag.UmpNInMobi, $"{Tag}->UpdateGdprConsent", nameof(consentObject), consentObject);
            using (var jsonObject = CreateJSONObject(consentObject))
            {
                _inMobiSdk.CallStatic("updateGDPRConsent", jsonObject);
            }
        }
    
        public void SetUSPrivacyString(string usPrivacyString)
        {
            this.LogInfo(SharedLogTag.UmpNInMobi, $"{Tag}->SetUSPrivacyString", nameof(usPrivacyString), usPrivacyString);
            _inMobiPrivacyCompliance.CallStatic("setUSPrivacyString", usPrivacyString);
        }

        private AndroidJavaObject CreateJSONObject(Dictionary<string, object> dictionary)
        {
            var jsonObject = new AndroidJavaObject("org.json.JSONObject");
            
            foreach (var kvp in dictionary)
            {
                try
                {
                    jsonObject.Call<AndroidJavaObject>("put", kvp.Key, kvp.Value);
                }
                catch (System.Exception e)
                {
                    this.LogError(SharedLogTag.UmpNInMobi, $"{Tag}->CreateJSONObject failed for key: {kvp.Key}", e.Message);
                }
            }
            
            return jsonObject;
        }
    }
}
#endif