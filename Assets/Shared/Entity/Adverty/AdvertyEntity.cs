using Newtonsoft.Json;

namespace Shared.Entity.Adverty
{
    [System.Serializable]
    public class AdvertyEntity
    {
        [JsonProperty("apiKey")] public string APIKey { get; set; }
        [JsonProperty("enableLogs")] public bool EnableLogs { get; set; }
        [JsonProperty("logLevel")] public string LogLevel { get; set; }
        [JsonProperty("apiVersion")] public string ApiVersion { get; set; }
        [JsonProperty("version")] public string Version { get; set; }
        [JsonProperty("sandboxMode")] public bool SandboxMode { get; set; }
    }
}