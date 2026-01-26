using Newtonsoft.Json;

namespace Shared.Entity.InAppUpdate
{
    [System.Serializable]
    public class InAppUpdateConfig
    {
        [JsonProperty("Unlocked")] public bool Unlocked { get; private set; }
        [JsonProperty("MinVersionCode")] public int MinVersionCode { get; private set; }
    }
}

