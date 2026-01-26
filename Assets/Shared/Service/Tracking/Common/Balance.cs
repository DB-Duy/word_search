using Newtonsoft.Json;

namespace Shared.Service.Tracking.Common
{
    [System.Serializable]
    public class Balance
    {
        [JsonProperty("key")] public string Key { get; }
        [JsonProperty("v")] public long Value { get; }

        public Balance(string key, long value)
        {
            Key = key;
            Value = value;
        }
    }
}