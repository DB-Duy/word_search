using System.Collections.Generic;
using Shared.Core.Repository.RemoteConfig;
using Shared.Utils;
using UnityEngine;

namespace Shared.Service.FirebaseRemoteConfig
{
    public static class FirebaseRemoteConfigRegistry
    {
        private static Dictionary<string, IFirebaseRemoteConfigRepository> _repositories = new();
        public static void Register(IFirebaseRemoteConfigRepository repository)
        {
            _repositories.Upsert(repository.Name, repository);
        }
        
        public static void OnValueChanged(string key)
        {
            if (_repositories.TryGetValue(key, out var repository))
                repository.OnValueChangedEvent?.Invoke();
            else
                Debug.LogWarning($"{SharedLogTag.FirebaseNConfig} FirebaseRemoteConfigRegistry->OnValueChanged { StringUtils.ToJsonString("key", key, "reason", "_repositories doesn't contains such key")}");
        }
    }
}