using Shared.Service.Tracking.Common;

namespace Shared.Service.Crash
{
    public interface ICrashService
    {
        CrashReason GetCrashReason();
    }
}