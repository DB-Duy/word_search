#if HUAWEI
using HmsPlugin;
using Newtonsoft.Json;
using Shared.Utils;
using UnityEngine;

namespace Shared.PlayerPrefsRepository.RemoteConfig
{
    public class HuaweiRemoteConfigRepository<T> : IRemoteConfigRepository<T>
    {
        private const string TAG = "HuaweiRemoteConfigRepository";

        public string Name { get; }

        private readonly T _defaultValue;

        public HuaweiRemoteConfigRepository(string name, T defaultValue)
        {
            if (string.IsNullOrEmpty(name)) throw new System.Exception($"{TAG}->Constructor: Invalid name={name}");
            Name = name;
            _defaultValue = defaultValue;
        }

        public T Get()
        {
            if (Application.isEditor) return _defaultValue;
            var val = HMSRemoteConfigManager.Instance.GetValueAsString(Name);
            SharedLogger.Log($"{TAG}->Get: {Name}={val}");
            return _ValidateExisted(val) ? JsonConvert.DeserializeObject<T>(val) : _defaultValue;
        }

        public bool IsExisted()
        {
            if (Application.isEditor) return true;
            var val = HMSRemoteConfigManager.Instance.GetValueAsString(Name);
            return _ValidateExisted(val);
        }

        private bool _ValidateExisted(string value) => !string.IsNullOrEmpty(value);
    }
}
#endif