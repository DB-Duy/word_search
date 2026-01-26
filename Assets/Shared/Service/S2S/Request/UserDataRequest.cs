using Newtonsoft.Json;

namespace Shared.Service.S2S.Request
{
    [System.Serializable]
    public class UserDataRequest
    {
        [JsonProperty("userId")] public string UserId { get; set; }
        [JsonProperty("adid")] public string AdjustId { get; set; }
        [JsonProperty("gps_adid")] public string GpsAdId { get; set; }
        [JsonProperty("android_id")] public string AndroidId { get; set; }
        [JsonProperty("firebaseAppInstanceId")] public string FirebaseAppInstanceId { get; set; }
        [JsonProperty("appMetricaProfileId")] public string AppMetricaProfileId { get; set; }
    }
}