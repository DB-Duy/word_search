#if UNITY_IOS && FYBER
using System.Runtime.InteropServices;
using Shared.Core.IoC;
using Shared.Utils;

namespace Shared.Service.Fyber.IOS
{
    [Component]
    public class IOSFyber : ISharedFyber, ISharedUtility
    {
        [DllImport("__Internal")]
        private static extern void Fyber_setGDPRConsentString(string gdprConsentString);

        [DllImport("__Internal")]
        private static extern void Fyber_setGDPRConsent(string v);

        [DllImport("__Internal")]
        private static extern void Fyber_setCCPAString(string ccpaString);

        [DllImport("__Internal")]
        private static extern void Fyber_clearCCPAString();

        [DllImport("__Internal")]
        private static extern void Fyber_log(string logString);

        public void ClearGdprConsentData()
        {
            this.LogWarning(SharedLogTag.UmpNFyber, "status", "NOT_IMPLEMENT");
        }

        public void ClearUsPrivacyString()
        {
            this.LogWarning(SharedLogTag.UmpNFyber, "status", "NOT_IMPLEMENT");
        }

        public void SetGdprConsent(bool value)
        {
            this.LogInfo(SharedLogTag.UmpNFyber, nameof(value), value);
            Fyber_setGDPRConsent(value ? "true" : "false");
        }

        public void SetGdprConsentString(string consentString)
        {
            this.LogInfo(SharedLogTag.UmpNFyber, nameof(consentString), consentString);
            Fyber_setGDPRConsentString(consentString);
        }

        public void SetUsPrivacyString(string value)
        {
            this.LogInfo(SharedLogTag.UmpNFyber, nameof(value), value);
            Fyber_setCCPAString(value);
        }
    }
}
#endif