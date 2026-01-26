#if DEV_TO_DEV && FIREBASE
using System.Collections.Generic;

namespace Shared.PlayerPrefsRepository.RemoteConfig
{
    public class D2DFirebaseRemoteConfigRepository<T> : IRemoteConfigRepository<T>
    {
        // ReSharper disable once InconsistentNaming
        private const string TAG = "D2DFirebaseRemoteConfigRepository";

        public string Name { get; }
        private readonly T _defaultValue;
        private readonly List<IRemoteConfigRepository<T>> _repositories = new();

        
        public D2DFirebaseRemoteConfigRepository(string name, IDefaultRemoteConfigController defaultRemoteConfigController)
        {
            if (string.IsNullOrEmpty(name)) throw new System.Exception($"{TAG}->Construction: Invalid name={name}");
            Name = name;
            _defaultValue = defaultRemoteConfigController.Get<T>(name);
            _repositories.Add(new D2DRemoteConfigRepository<T>(name, defaultValue: default));
            _repositories.Add(new FirebaseRemoteConfigRepository<T>(name, defaultValue: _defaultValue));
        }
        
        public D2DFirebaseRemoteConfigRepository(string name, T defaultValue)
        {
            if (string.IsNullOrEmpty(name)) throw new System.Exception($"{TAG}->Construction: Invalid name={name}");
            Name = name;
            _defaultValue = defaultValue;
            _repositories.Add(new D2DRemoteConfigRepository<T>(name, defaultValue: default(T)));
            _repositories.Add(new FirebaseRemoteConfigRepository<T>(name, defaultValue: defaultValue));
        }

        public T Get()
        {
            foreach (var r in _repositories)
            {
                if (r.IsExisted()) return r.Get();
            }

            return _defaultValue;
        }

        public bool IsExisted()
        {
            foreach (var r in _repositories)
            {
                if (r.IsExisted()) return true;
            }
            return false;
        }
    }
}
#endif