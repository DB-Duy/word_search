using Newtonsoft.Json;

namespace Shared.SharedReport
{
    [System.Serializable]
    public class VersionReportConfig
    {
        [JsonProperty("endpoint")] public string Endpoint { get; private set; }
        [JsonProperty("api_key")] public string ApiKey { get; private set; }

        public static VersionReportConfig NewInstance(string endpoint, string apikey)
        {
            return new VersionReportConfig()
            {
                Endpoint = endpoint,
                ApiKey = apikey
            };
        }
    }
}