using Newtonsoft.Json;

namespace Shared.Core.Handler.Corou.Initializable
{
    [System.Serializable]
    public class Config
    {
        [JsonProperty("name")] public string Name { get; set; }
        [JsonProperty("timeout")] public int TimeOut { get; set; } = 0;
        [JsonProperty("freeTask")] public bool IsFreeTask { get; set; } = false;
        [JsonProperty("optional")] public bool Optional { get; set; } = false;
    }
}