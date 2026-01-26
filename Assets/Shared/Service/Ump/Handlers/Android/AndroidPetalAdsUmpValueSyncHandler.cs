#if HUAWEI && UNITY_ANDROID
using Shared.SharedPetalAds;
using Shared.UMP;
using Shared.Ump.Handlers;
using Shared.Utils;
using UnityEngine;

namespace Shared.Ump.Android
{
    public class AndroidPetalAdsUmpValueSyncHandler : IUmpValueSyncHandler
    {
        // ReSharper disable once InconsistentNaming
        private const string TAG = "AndroidPetalAdsUmpValueSyncHandler";
        
        public void OnValueSyncEvent(IUmpController controller, bool gdpr, string consentString, string usPrivacy)
        {
            SharedLogger.Log($"{TAG}->OnValueSyncEvent: gdpr={gdpr}, consentString={consentString}, usPrivacy={usPrivacy}", UmpConst.UMP_LOG_FILTER);
            var binder = new AndroidJavaClass("com.unity3d.player.SharedPetalAds");
            binder.CallStatic("setConsent", consentString);
            SharedPetalAdsImpl.Debug();
        }
    }
}
#endif