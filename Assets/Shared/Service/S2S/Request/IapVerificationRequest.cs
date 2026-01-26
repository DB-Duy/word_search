using Newtonsoft.Json;

namespace Shared.Service.S2S.Request
{
    [System.Serializable]
    public class IapVerificationRequest
    {
        [JsonProperty("userData")] public UserDataRequest UserData { get; set; }
        [JsonProperty("receipt")] public string Receipt { get; set; }
    }
}