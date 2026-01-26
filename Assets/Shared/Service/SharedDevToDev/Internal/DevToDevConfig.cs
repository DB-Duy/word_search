using System.Collections.Generic;
using Newtonsoft.Json;

namespace Shared.SharedDevToDev.Internal
{
    [System.Serializable]
    public class DevToDevConfig : IDevToDevConfig
    {
        [JsonProperty("AppId")] public string AppId { get; }
        [JsonProperty("Level")] public int Level { get; }
        [JsonProperty("DefaultRemoteConfigDictionary")] public Dictionary<string, object> DefaultRemoteConfigDictionary { get; }
        [JsonProperty("StrictMode")] public bool StrictMode { get; }

        public DevToDevConfig(string appId, int level, Dictionary<string, object> defaultRemoteConfigDictionary, bool strictMode)
        {
            AppId = appId;
            Level = level;
            DefaultRemoteConfigDictionary = defaultRemoteConfigDictionary;
            StrictMode = strictMode;
        }

        public override string ToString() => JsonConvert.SerializeObject(this);
    }
}