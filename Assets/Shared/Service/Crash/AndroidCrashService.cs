#if UNITY_ANDROID && !UNITY_EDITOR

using Shared.Core.IoC;
using Shared.Service.Crash.Internal;
using Shared.Service.Tracking.Common;
using UnityEngine;

namespace Shared.Service.Crash
{
    [Service]
    public class AndroidCrashService : ICrashService
    {
        private AndroidCrashReasonResolver _androidCrashReasonResolver = new();
        private static int _GetSDKInt()
        {
            if (Application.isEditor) return 29;
            using var version = new AndroidJavaClass("android.os.Build$VERSION");
            return version.GetStatic<int>("SDK_INT");
        }

        private static int _GetExitReason()
        {
            var reason = -1;
            if (_GetSDKInt() < 30) return reason;
            var jc = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            var jo = jc.GetStatic<AndroidJavaObject>("currentActivity");

            var binder = new AndroidJavaClass("com.unity3d.player.CrashReporter");
            reason = binder.CallStatic<int>("getExitReason", jo);
            return reason;
        }
        
        public CrashReason GetCrashReason() => _androidCrashReasonResolver.Resolve(_GetExitReason());
    }
}
#endif