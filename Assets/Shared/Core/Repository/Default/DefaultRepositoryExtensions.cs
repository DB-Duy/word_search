using System;
using Shared.Utils;

namespace Shared.Core.Repository.Default
{
    public static class DefaultRepositoryExtensions
    {
        // ----------------------------------------------------------------------------------------
        // Repository
        // ----------------------------------------------------------------------------------------
        private static IDefaultRepository _defaultRepository;
        private static IDefaultRepository DefaultRepository => _defaultRepository ??= new DefaultRepository(isRemoteConfig: false);

        public static string GetRepositoryName(this IDefaultRepositoryExtensions u, Type type) => DefaultRepository.GetName(type);
        public static bool GetRepositoryBool(this IDefaultRepositoryExtensions u, Type type) => DefaultRepository.GetBool(type);
        public static string GetRepositoryString(this IDefaultRepositoryExtensions u, Type type) => DefaultRepository.GetString(type);
        public static int GetRepositoryInt(this IDefaultRepositoryExtensions u, Type type) => DefaultRepository.GetInt(type);
        public static long GetRepositoryLong(this IDefaultRepositoryExtensions u, Type type) => DefaultRepository.GetLong(type);
        public static T GetRepositoryObject<T>(this IDefaultRepositoryExtensions u, Type type) => DefaultRepository.GetObject<T>(type);
        
        // ----------------------------------------------------------------------------------------
        // Remote Config Repository
        // ----------------------------------------------------------------------------------------
        private static IDefaultRepository _defaultRemoteConfigRepository;
        private static IDefaultRepository DefaultRemoteConfigRepository => _defaultRemoteConfigRepository ??= new DefaultRepository(isRemoteConfig: true);

        public static string GetRemoteConfigRepositoryName(this IDefaultRepositoryExtensions u, Type type) => DefaultRemoteConfigRepository.GetName(type);
        // public static bool GetRemoteConfigRepositoryBool(this IDefaultRepositoryExtensions u, Type type) => DefaultRemoteConfigRepository.GetBool(type);
        // public static string GetRemoteConfigRepositoryString(this IDefaultRepositoryExtensions u, Type type) => DefaultRemoteConfigRepository.GetString(type);
        // public static int GetRemoteConfigRepositoryInt(this IDefaultRepositoryExtensions u, Type type) => DefaultRemoteConfigRepository.GetInt(type);
        // public static long GetRemoteConfigRepositoryLong(this IDefaultRepositoryExtensions u, Type type) => DefaultRemoteConfigRepository.GetLong(type);
        // public static T GetRemoteConfigRepositoryObject<T>(this IDefaultRepositoryExtensions u, Type type) => DefaultRemoteConfigRepository.GetObject<T>(type);
        public static string GetRemoteConfigRepositoryString(this IDefaultRepositoryExtensions u, Type type)
        {
#if LOG_INFO
            var n = DefaultRemoteConfigRepository.GetName(type);
            var v = DefaultRemoteConfigRepository.GetString(type);
            // SharedLogger.LogInfoCustom(SharedLogTag.RemoteConfig, "DefaultRepositoryExtensions", "GetRemoteConfigRepositoryString", "from", "RemoteConfigConfigurations.json", "name", n, "value", v);
            return v;
#else
            return DefaultRemoteConfigRepository.GetString(type);
#endif
        }
    }
}