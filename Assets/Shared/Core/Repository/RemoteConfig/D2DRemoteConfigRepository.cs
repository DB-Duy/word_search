#if DEV_TO_DEV
using DevToDev.Analytics.ABTest;
using Newtonsoft.Json;
using Shared.Utils;

namespace Shared.PlayerPrefsRepository.RemoteConfig
{
    public class D2DRemoteConfigRepository<T> : IRemoteConfigRepository<T>
    {
        const string TAG = "D2DRemoteConfigRepository";

        public string Name { get; }
        private readonly T _defaultValue;

        public D2DRemoteConfigRepository(string name, IDefaultRemoteConfigController defaultRemoteConfigController)
        {
            if (string.IsNullOrEmpty(name)) throw new System.Exception($"{TAG}->D2DRemoteConfigRepository: Invalid name={name}");
            Name = name;
            _defaultValue = defaultRemoteConfigController.Get<T>(name);
        }
        
        public D2DRemoteConfigRepository(string name, T defaultValue)
        {
            if (string.IsNullOrEmpty(name)) throw new System.Exception($"{TAG}->D2DRemoteConfigRepository: Invalid name={name}");
            Name = name;
            _defaultValue = defaultValue;
        }

        public T Get()
        {
            if (!IsExisted()) return _defaultValue;
            var config = DTDRemoteConfig.Config[Name];
            var val = config.StringValue();
            SharedLogger.Log($"{TAG}->DTDRemoteConfig->Get: {Name}={val}");
            return string.IsNullOrEmpty(val) ? _defaultValue : JsonConvert.DeserializeObject<T>(val);
        }

        public bool IsExisted()
        {
            if (DTDRemoteConfig.Config == null) return false;
            if (!DTDRemoteConfig.Config.HasKey(Name)) return false;
            if (DTDRemoteConfig.Config[Name] == null) return false;
            var v = DTDRemoteConfig.Config[Name].StringValue();
            return !string.IsNullOrEmpty(v);
        }
    }
}
#endif