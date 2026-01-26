using System;
using Newtonsoft.Json;

namespace Shared.Entity.Ump
{
    [Serializable]
    public class UmpEntity
    {
        [JsonProperty("gdprApplies")] public int GdprApplies { get; set; }

        [JsonProperty("TCString")] public string TcString { get; set; }

        [JsonProperty("PurposeConsents")] public string PurposeConsents { get; set; }

        [JsonProperty("UsPrivacyString")] public string UsPrivacyString { get; set; }

        [JsonProperty("GdprApply")] public bool GdprApply => GdprApplies == 1;
    }
}