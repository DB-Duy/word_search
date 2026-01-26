#if ODEEO_AUDIO
using MolocoSdk;
using Odeeo;
using Shared.Utils;

namespace Shared.Service.Odeeo
{
    public static class OdeeoUtilities
    {
        private const string Tag = "OdeeoUtilities";
        
        // public static void SetGdprConsent(bool consent, string consentString)
        // {
        //     SharedLogger.LogInfoCustom(SharedLogTag.UmpNOdeeo, Tag, "SetGdprConsent", nameof(consent), consent, nameof(consentString), consentString);
        //     //OdeeoSdk.SetGdprConsent(consent, consentString);
        //
        //     if ()
        //     {
        //         
        //     }
        // }
        
        public static void SetGdprConsent(bool consent)
        {
            SharedLogger.LogInfoCustom(SharedLogTag.UmpNOdeeo, Tag, "SetGdprConsent", nameof(consent), consent);
            OdeeoSdk.ForceRegulationType(OdeeoSdk.ConsentType.Gdpr);
        }
        
        public static void SetDoNotSell(bool isApplied, string privacyString)
        {
            SharedLogger.LogInfoCustom(SharedLogTag.UmpNOdeeo, Tag, "SetDoNotSell", nameof(isApplied), isApplied, nameof(privacyString), privacyString);
            OdeeoSdk.SetDoNotSell(isApplied, privacyString);
        }
    }
}
#endif