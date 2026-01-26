#if ADJUST && UNITY_ANDROID
using Shared.Core.IoC;
using Shared.Utils;
using UnityEngine;

namespace Shared.Service.SharedAdjust.Handler
{
    [Component]
    public class AndroidVivoReferrerHandler : IAdjustReferrerHandler, ISharedUtility
    {
        public void Handle()
        {
            this.LogInfo(SharedLogTag.AdjustNReferrerNVivo);
            if (Application.isEditor) return;
            var referrer = new AndroidJavaClass("com.adjust.sdk.vivo.AdjustVivoReferrer");
            var currentActivity = new AndroidJavaClass("com.unity3d.player.UnityPlayer").GetStatic<AndroidJavaObject>("currentActivity");
            referrer.CallStatic("readVivoReferrer", currentActivity);
        }
    }
}
#endif