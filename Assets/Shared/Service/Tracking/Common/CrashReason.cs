namespace Shared.Service.Tracking.Common
{
    [System.Serializable]
    public class CrashReason : ValueObject
    {
        private CrashReason(string v) : base(v)
        {
        }

        public static readonly CrashReason None = new("null");
        public static readonly CrashReason Crash = new("crash");
        public static readonly CrashReason Anr = new("anr");
        public static readonly CrashReason CrashNative = new("crash_native");
        public static readonly CrashReason DependencyDied = new("dependency_died");
        public static readonly CrashReason ExcessiveResourceUsage = new("excessive_resource_usage");
        public static readonly CrashReason ExitSelf = new("exit_self");
        public static readonly CrashReason Freezer = new("freezer");
        public static readonly CrashReason InitializationFailure = new("initialization_failure");
        public static readonly CrashReason LowMemory = new("low_memory");
        public static readonly CrashReason Other = new("other");
        public static readonly CrashReason PermissionChange = new("permission_change");
        public static readonly CrashReason Signaled = new("signaled");
        public static readonly CrashReason Unknown = new("unknown");
        public static readonly CrashReason UserRequested = new("user_requested");
        public static readonly CrashReason UserStopped = new("user_stopped");
        public static readonly CrashReason FgNormalAppExit = new("fg_normal_app_exit");
        public static readonly CrashReason FgAbnormalExit = new("fg_abnormal_exit");
        public static readonly CrashReason FgAppWatchdogExit = new("fg_app_watchdog_exit");
        public static readonly CrashReason FgMemoryResourceLimitExit = new("fg_memory_resource_limit_exit");
        public static readonly CrashReason FgBadAccessExit = new("fg_bad_access_exit");
        public static readonly CrashReason FgIllegalInstructionExit = new("fg_illegal_instruction_exit");
        public static readonly CrashReason BgNormalAppExit = new("bg_normal_app_exit");
        public static readonly CrashReason BgAbnormalExit = new("bg_abnormal_exit");
        public static readonly CrashReason BgAppWatchdogExit = new("bg_app_watchdog_exit");
        public static readonly CrashReason BgCPUResourceLimitExit = new("bg_cpu_resource_limit_exit");
        public static readonly CrashReason BgMemoryResourceLimitExit = new("bg_memory_resource_limit_exit");
        public static readonly CrashReason BgMemoryPressureExit = new("bg_memory_pressure_exit");
        public static readonly CrashReason BgSuspendedWithLockedFileExit = new("bg_suspended_with_locked_file_exit");
        public static readonly CrashReason BgBadAccessExit = new("bg_bad_access_exit");
        public static readonly CrashReason BgIllegalInstructionExit = new("bg_illegal_instruction_exit");
        public static readonly CrashReason BgBackgroundTaskAssertionTimeoutExit = new("bg_background_task_assertion_timeout_exit");
    }
}