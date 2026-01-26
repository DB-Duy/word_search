using Newtonsoft.Json;

namespace Shared.Entity.AppMetrica
{
    [System.Serializable]
    public class AppMetricaEntity
    {
        [JsonProperty("user_profile_id")] public string UserProfileID { get; set; }
    }
}