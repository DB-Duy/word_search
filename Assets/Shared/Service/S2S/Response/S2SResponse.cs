using Newtonsoft.Json;

namespace Shared.Service.S2S.Response
{
    // Form upload complete! {"status":{"message":"Success","code":200,"success":true},"entity":{"isProduction":false,"errorMessage":null}}
    [System.Serializable]
    public class S2SResponse<T>
    {
        [System.Serializable]
        public class StatusImpl
        {
            [JsonProperty("message")] public string Message { get; }
            [JsonProperty("code")] public int Code { get; }
            [JsonProperty("success")] public bool Success { get; }
        }

        [JsonProperty("status")] private StatusImpl _statusImpl;
        [JsonProperty("entity")] public T Data { get; }

        public static S2SResponse<T> NewInstance<T>(string jsonString)
        {
            return JsonConvert.DeserializeObject<S2SResponse<T>>(jsonString);
        }
    }
}