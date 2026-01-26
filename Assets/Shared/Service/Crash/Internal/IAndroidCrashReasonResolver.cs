using Shared.Service.Tracking.Common;

namespace Shared.Service.Crash.Internal
{
    public interface IAndroidCrashReasonResolver
    {
        CrashReason Resolve(int code);
    }
}