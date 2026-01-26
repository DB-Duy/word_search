using Newtonsoft.Json;

namespace Shared.Entity.InAppUpdate
{
    [System.Serializable]
    public class InAppUpdateEntity
    {
        [JsonProperty("session_id")] public long SessionId { get; set; }
    }
}