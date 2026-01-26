#if GOOGLE_PLAY
using Newtonsoft.Json;

namespace Shared.Referrer
{
    [System.Serializable]
    public class ReferrerDetails
    {
        [JsonProperty("googlePlayInstantParam")] public bool GooglePlayInstantParam { get; private set; }

        [JsonProperty("installReferrer")] public string InstallReferrer { get; private set; }
        [JsonProperty("installBeginTimestampSeconds")] public long InstallBeginTimestampSeconds { get; private set; }
        [JsonProperty("installBeginTimestampServerSeconds")] public long InstallBeginTimestampServerSeconds { get; private set; }
        [JsonProperty("installVersion")] public string InstallVersion { get; private set; }
        
        [JsonProperty("referrerClickTimestampSeconds")] public long ReferrerClickTimestampSeconds { get; private set; }
        [JsonProperty("referrerClickTimestampServerSeconds")] public long ReferrerClickTimestampServerSeconds { get; private set; }

        public override string ToString() => JsonConvert.SerializeObject(this);
    }
}
#endif