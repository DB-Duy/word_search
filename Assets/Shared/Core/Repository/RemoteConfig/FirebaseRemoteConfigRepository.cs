using System;
using Newtonsoft.Json;
using Shared.Core.Repository.Default;
using Shared.Utils;

#if FIREBASE
using Firebase.RemoteConfig;
using Shared.Service.Firebase;
#endif


namespace Shared.Core.Repository.RemoteConfig
{
    public class FirebaseRemoteConfigRepository<T> : IRemoteConfigRepository<T>, IDefaultRepositoryExtensions, ISharedUtility, IFirebaseRemoteConfigRepository
    {
        public Action OnValueChangedEvent { get; set; }
        
        public string Name { get; }
        private T _currentValue;
        private string _currentValueString;

        public FirebaseRemoteConfigRepository()
        {
            Name = this.GetRemoteConfigRepositoryName(GetType());
        }

        public FirebaseRemoteConfigRepository(string name)
        {
            if (string.IsNullOrEmpty(name)) throw new System.Exception($"FirebaseRemoteConfigRepository->Constructor: Invalid name={name}");
            Name = name;
        }

        public T Get()
        {
#if FIREBASE
            var val = string.Empty;
            if (FirebaseFlag.IsEnabled)
                val = FirebaseRemoteConfig.DefaultInstance.GetValue(Name).StringValue;
            if (string.IsNullOrEmpty(val.Trim())) 
                val = this.GetRemoteConfigRepositoryString(GetType());
            if (string.IsNullOrEmpty(val)) return default;
            if (val == _currentValueString && _currentValue != null) return _currentValue;
            _currentValueString = val;
            _currentValue = JsonConvert.DeserializeObject<T>(_currentValueString);
            this.LogInfo(SharedLogTag.RemoteConfig, nameof(Name), Name, nameof(_currentValueString), _currentValueString);
            return _currentValue;
#else
            var val = this.GetRemoteConfigRepositoryString(GetType());
            if (string.IsNullOrEmpty(val)) return default;
            if (val == _currentValueString && _currentValue != null) return _currentValue;
            _currentValueString = val;
            _currentValue = JsonConvert.DeserializeObject<T>(_currentValueString);
            this.LogInfo(SharedLogTag.RemoteConfig, nameof(Name), Name, nameof(_currentValueString), _currentValueString);
            return _currentValue;
#endif
        }
    }
}