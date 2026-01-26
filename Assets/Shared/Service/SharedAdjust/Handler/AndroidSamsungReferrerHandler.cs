#if ADJUST && UNITY_ANDROID
using Shared.Core.IoC;
using Shared.Utils;
using UnityEngine;

namespace Shared.Service.SharedAdjust.Handler
{
    [Component]
    public class AndroidSamsungReferrerHandler : IAdjustReferrerHandler, ISharedUtility
    {
        public void Handle()
        {
            this.LogInfo(SharedLogTag.AdjustNReferrerNSamsung);
            if (Application.isEditor) return;
            var referrer = new AndroidJavaClass("com.adjust.sdk.samsung.AdjustSamsungReferrer");
            var currentActivity = new AndroidJavaClass("com.unity3d.player.UnityPlayer").GetStatic<AndroidJavaObject>("currentActivity");
            referrer.CallStatic("readSamsungReferrer", currentActivity);
        }
    }
}
#endif