#if USING_UMP
using System;
using GoogleMobileAds.Ump.Api;

namespace Shared.Service.Ump.Internal
{
    [System.Serializable]
    public class UmpProcessInfo
    {
        public IUmpService Service { get; }
        public float TimeOutInSeconds { get; }
        public ConsentRequestParameters ConsentRequestParameters { get; }
        public ConsentForm ConsentForm { get; set; }
        public Action OnComplete { get; set; }

        public UmpProcessInfo(IUmpService service, float timeOutInSeconds = 0, ConsentDebugSettings consentDebugSettings = null)
        {
            Service = service;
            TimeOutInSeconds = timeOutInSeconds;
            ConsentRequestParameters = new ConsentRequestParameters
            {
                TagForUnderAgeOfConsent = false
            };
#if LOG_INFO
            if (consentDebugSettings != null) ConsentRequestParameters.ConsentDebugSettings = consentDebugSettings;
#endif
        }
    }
}
#endif