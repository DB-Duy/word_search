#if HUAWEI && BIGO
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Newtonsoft.Json;
using Shared.Utils;
using UnityEngine;

namespace Shared.Ads.SharedTopOn.PreHandler
{
    public class PreBigoHandler : IPreInitHandler
    {
        private const string TAG = "PreBigoHandler";

#if UNITY_ANDROID
        private readonly AndroidJavaClass _javaClass = new($"{BigoAds.Scripts.Platforms.Android.AndroidPlatformTool.ClassPackage}.BigoAdSdk");
        //Fixed Android memory leak (global reference table overflow (max=51200)global reference table dump)
        private bool _IsInitSuccess() => _javaClass.CallStatic<bool>("isInitialized");
#endif
        
        public IEnumerator Handle()
        {
#if UNITY_ANDROID
            yield return new WaitUntil(_IsInitSuccess);
#else
            yield return new WaitUntil(() => BigoAds.Scripts.Api.BigoAdSdk.IsInitSuccess());
#endif
            var userConsent = true;
#if USING_UMP
            var usPrivacy = SharedCore.Instance.UmpController.IABUSPrivacy_String;
            userConsent = string.IsNullOrEmpty(usPrivacy) || usPrivacy.Equals(Ump.UmpConst.CONST_1YNN_OFF);
#endif
            BigoAds.Scripts.Api.BigoAdSdk.SetUserConsent(BigoAds.Scripts.Api.Constant.ConsentOptions.GDPR, true);
            BigoAds.Scripts.Api.BigoAdSdk.SetUserConsent(BigoAds.Scripts.Api.Constant.ConsentOptions.CCPA, userConsent);
        }
    }
}
#endif