using Newtonsoft.Json;

namespace Shared.Service.Ads.Common
{
    [System.Serializable]
    public class RewardedAdOperation
    {
        [JsonProperty("placement")] public string Placement { get; }
        [JsonProperty("is_rewarded")] public bool IsRewarded { get; set; }
        [JsonProperty("is_closed")] public bool IsClosed { get; set; }
        [JsonProperty("localized_error_message")] public string LocalizedErrorMessage { get; set; }

        public RewardedAdOperation(string placement, string localizedErrorMessage = null)
        {
            Placement = placement;
            LocalizedErrorMessage = localizedErrorMessage;
        }
    }
}