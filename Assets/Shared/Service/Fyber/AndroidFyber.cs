#if UNITY_ANDROID && FYBER

using Shared.Utils;
using UnityEngine;

namespace Shared.Service.Fyber
{

    /// <summary>
    /// https://developer.digitalturbine.com/hc/en-us/articles/360010822437-Integrating-the-Android-SDK
    /// https://developer.digitalturbine.com/hc/en-us/articles/360010915618-Integrating-the-iOS-SDK
    /// </summary>
    public class AndroidFyber : ISharedFyber, ISharedUtility
    {
        private const string Tag = "AndroidFyber";

        private readonly AndroidJavaClass _adManager = new("com.fyber.inneractive.sdk.external.InneractiveAdManager");

        public void ClearGdprConsentData()
        {
            SharedLogger.LogJson(SharedLogTag.FyberNUmp, $"{Tag}->ClearGdprConsentData");
            _adManager.CallStatic("clearGdprConsentData");
        }

        public void ClearUsPrivacyString()
        {
            SharedLogger.LogJson(SharedLogTag.FyberNUmp, $"{Tag}->ClearUsPrivacyString");
            _adManager.CallStatic("clearUSPrivacyString");
        }

        public void SetGdprConsent(bool value)
        {
            this.LogInfo(SharedLogTag.UmpNFyber, nameof(value), value);
            _adManager.CallStatic("setGdprConsent", value);
        }

        public void SetGdprConsentString(string value)
        {
            this.LogInfo(SharedLogTag.UmpNFyber, nameof(value), value);
            _adManager.CallStatic("setGdprConsentString", value);
        }

        public void SetUsPrivacyString(string value)
        {
            this.LogInfo(SharedLogTag.UmpNFyber, nameof(value), value);
            _adManager.CallStatic("setUSPrivacyString", value);
        }
    }
}
#endif