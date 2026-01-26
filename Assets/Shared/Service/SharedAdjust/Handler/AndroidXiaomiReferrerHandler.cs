#if ADJUST && UNITY_ANDROID
using Shared.Core.IoC;
using Shared.Utils;
using UnityEngine;

namespace Shared.Service.SharedAdjust.Handler
{
    [Component]
    public class AndroidXiaomiReferrerHandler : IAdjustReferrerHandler, ISharedUtility
    {
        public void Handle()
        {
            this.LogInfo(SharedLogTag.AdjustNReferrerNXiaomi);
            if (Application.isEditor) return;
            var currentActivity = new AndroidJavaClass("com.unity3d.player.UnityPlayer").GetStatic<AndroidJavaObject>("currentActivity");
            var referrer = new AndroidJavaClass("com.adjust.sdk.xiaomi.AdjustXiaomiReferrer");
            referrer.CallStatic("readXiaomiReferrer", currentActivity);
        }
    }
}
#endif