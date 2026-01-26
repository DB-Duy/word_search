using Newtonsoft.Json;

namespace Shared.Service.S2S.Response
{
    // {"status":{"message":"Success","code":200,"success":true},"entity":{"isProduction":false,"errorMessage":null}}
    [System.Serializable]
    public class IapVerificationResponse
    {
        [JsonProperty("isProduction")] public bool IsProduction { get; set; }
        [JsonIgnore] public bool IsSandbox => !IsProduction;

    }
}