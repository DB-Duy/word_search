using Newtonsoft.Json;

namespace Shared.Entity.S2S
{
    [System.Serializable]
    public class S2SConfig
    {
        [JsonProperty("Unlocked")] public bool Unlocked { get; private set; }
        [JsonProperty("remote_url")] public string RemoteUrl { get; private set; }
        [JsonProperty("api_key")] public string ApiKey { get; private set; }
        
        public static S2SConfig NewInstance(bool unlocked, string remoteUrl, string apiKey)
        {
            return new S2SConfig
            {
                Unlocked = unlocked,
                RemoteUrl = remoteUrl,
                ApiKey = apiKey
            };
        }

        public override string ToString() => JsonConvert.SerializeObject(this);
    }
}