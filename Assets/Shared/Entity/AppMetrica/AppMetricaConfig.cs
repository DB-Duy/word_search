using Newtonsoft.Json;

namespace Shared.Entity.AppMetrica
{
    [System.Serializable]
    public class AppMetricaConfig
    {
        [JsonProperty("Unlocked")] public bool Unlocked { get; private set; }
        [JsonProperty("session_timeout")] public int SessionTimeOut { get; private set; }

        // public static AppMetricaConfig NewClientDefault()
        // {
        //     var config = new AppMetricaConfig
        //     {
        //         Unlocked = true,
        //         SessionTimeOut = 300
        //     };
        //     return config;
        // }
    }
}