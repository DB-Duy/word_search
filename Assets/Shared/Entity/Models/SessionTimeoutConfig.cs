using Newtonsoft.Json;

namespace Shared.Repository.RemoteConfig.Models
{
    [System.Serializable]
    public class SessionTimeoutConfig
    {
        [JsonProperty("session_timeout")] public int SessionTimeout { get; private set; }

        public static SessionTimeoutConfig NewClientDefault()
        {
            var config = new SessionTimeoutConfig
            {
                SessionTimeout = 301
            };
            return config;
        }
        public override string ToString() => JsonConvert.SerializeObject(this);
    }
}