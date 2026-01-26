#if UNITY_EDITOR || UNITY_IOS

using Shared.Core.IoC;
using Shared.Service.Tracking.Common;

namespace Shared.Service.Crash
{
    [Service]
    public class DoNotSupportCrashService : ICrashService
    {
        public CrashReason GetCrashReason() => CrashReason.None;
    }
}

#endif