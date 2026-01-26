using System.Collections.Generic;
using System.Diagnostics;
using Newtonsoft.Json;
using Shared.Utils;

namespace Shared.SharedPetalAds
{
    public class SharedPetalAdsImpl
    {
        private const string TAG = "SharedPetalAdsImpl";
        
#if HUAWEI && UNITY_ANDROID
        private static readonly ISharedPetalAds _instance = new SharedAndroidPetalAds();
#else
        private static readonly ISharedPetalAds _instance = null;
#endif
        
        public static string Consent => _instance == null ? "Consent: Not Implemented" : _instance.Consent;
        public static string SdkVersion => _instance == null ? "SdkVersion: Not Implemented" : _instance.SdkVersion;

        [Conditional("LOG_INFO")]
        public static void Debug()
        {
            Dictionary<string, object> logDict = new()
            {
                {"Consent", Consent},
                {"SdkVersion", SdkVersion}
            };
            SharedLogger.Log($"{TAG}->Debug: {JsonConvert.SerializeObject(logDict)}");
        }
    }
}