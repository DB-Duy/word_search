#if FIREBASE_WEBGL
using Newtonsoft.Json;
using Shared.Utils;
using System.Runtime.InteropServices;

namespace Shared.PlayerPrefsRepository.RemoteConfig
{
    namespace Shared.PlayerPrefsRepository.RemoteConfig
    {
        class WebFirebaseRemoteConfigBridge
        {
            [DllImport("__Internal")]
            private static extern string GetFirebaseRemoteConfigValue(string name);
            public string Get(string key)
            {
                var val = GetFirebaseRemoteConfigValue(key);
                return val;
            }

        }
        public class WebFirebaseRemoteConfigRepository <T> : IRemoteConfigRepository<T>
        {
            const string TAG = "WebFirebaseRemoteConfigRepository";

            public string Name { get; }
            private readonly T _defaultValue;
            private readonly WebFirebaseRemoteConfigBridge bridge = new();

            public WebFirebaseRemoteConfigRepository(string name, T defaultValue)
            {
                if (string.IsNullOrEmpty(name)) throw new System.Exception($"{TAG}->Constructure: Invalid name={name}");
                Name = name;
                _defaultValue = defaultValue;
            }

            public T Get()
            {
#if UNITY_EDITOR
                return _defaultValue;
#else
                var val = bridge.Get(Name);
                SharedLogger.Log($"{TAG}->Get: {Name}={val}");
                return string.IsNullOrEmpty(val) ? _defaultValue : JsonConvert.DeserializeObject<T>(val);
#endif
            }

            public bool IsExisted()
            {
#if UNITY_EDITOR
                return true;
#else
                var val = bridge.Get(Name);
                return !string.IsNullOrEmpty(val);
#endif
            }
        }
    }
}
#endif