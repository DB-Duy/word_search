using Shared.Service.Tracking.Common;

namespace Shared.Service.Crash.Internal
{
    public class AndroidCrashReasonResolver : IAndroidCrashReasonResolver
    {
        private const int ReasonUnknown = 0;
        private const int ReasonExitSelf = 1;
        private const int ReasonSignaled = 2;
        private const int ReasonLowMemory = 3;
        private const int ReasonCrash = 4;
        private const int ReasonCrashNative = 5;
        private const int ReasonAnr = 6;
        private const int ReasonInitializationFailure = 7;
        private const int ReasonPermissionChange = 8;
        private const int ReasonExcessiveResourceUsage = 9;
        private const int ReasonUserRequested = 10;
        private const int ReasonUserStopped = 11;
        private const int ReasonDependencyDied = 12;
        private const int ReasonOther = 13;

        public CrashReason Resolve(int code)
        {
            return code switch
            {
                ReasonUnknown => CrashReason.Unknown,
                ReasonExitSelf => CrashReason.ExitSelf,
                ReasonSignaled => CrashReason.Signaled,
                ReasonLowMemory => CrashReason.LowMemory,
                ReasonCrash => CrashReason.Crash,
                ReasonCrashNative => CrashReason.CrashNative,
                ReasonAnr => CrashReason.Anr,
                ReasonInitializationFailure => CrashReason.InitializationFailure,
                ReasonPermissionChange => CrashReason.PermissionChange,
                ReasonExcessiveResourceUsage => CrashReason.ExcessiveResourceUsage,
                ReasonUserRequested => CrashReason.UserRequested,
                ReasonUserStopped => CrashReason.UserStopped,
                ReasonDependencyDied => CrashReason.DependencyDied,
                ReasonOther => CrashReason.Other,
                _ => CrashReason.None
            };
        }
    }
}