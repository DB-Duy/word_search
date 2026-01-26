using Newtonsoft.Json;

namespace Shared.Entity.Firebase
{
    [System.Serializable]
    public class FirebaseEntity
    {
        [JsonProperty("analytics_instance_id")] public string AnalyticsInstanceId { get; set; }
    }
}