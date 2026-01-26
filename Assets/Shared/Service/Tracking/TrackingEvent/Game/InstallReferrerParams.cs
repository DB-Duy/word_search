using System.Collections.Generic;
using Newtonsoft.Json;
using Shared.Tracking.Templates;

namespace Shared.Tracking.Models.Game
{
    public class InstallReferrerParams : BaseTrackingEvent, IConvertableEvent
    {
        [JsonProperty("event_name")] public override string EventName => "install_referrer";
        
        [JsonProperty("googlePlayInstantParam")] public bool GooglePlayInstantParam { get; set; }

        [JsonProperty("installReferrer")] public string InstallReferrer { get; set; }
        [JsonProperty("installBeginTimestampSeconds")] public long InstallBeginTimestampSeconds { get; set; }
        [JsonProperty("installBeginTimestampServerSeconds")] public long InstallBeginTimestampServerSeconds { get; set; }
        [JsonProperty("installVersion")] public string InstallVersion { get; set; }
        
        [JsonProperty("referrerClickTimestampSeconds")] public long ReferrerClickTimestampSeconds { get; set; }
        [JsonProperty("referrerClickTimestampServerSeconds")] public long ReferrerClickTimestampServerSeconds { get; set; }

        [JsonIgnore] private Dictionary<string, object> _params;

        public override string ToString() => JsonConvert.SerializeObject(ToConvertableEvent());

        public Dictionary<string, object> ToConvertableEvent()
        {
            return _params ??= new Dictionary<string, object>(ExParams)
            {
                    {"googlePlayInstantParam", GooglePlayInstantParam},
                    {"installReferrer", InstallReferrer ?? "null"},
                    {"installBeginTimestampSeconds", InstallBeginTimestampSeconds},
                    {"installBeginTimestampServerSeconds", InstallBeginTimestampServerSeconds},
                    {"installVersion", InstallVersion ?? "null"},
                    {"referrerClickTimestampSeconds", ReferrerClickTimestampSeconds},
                    {"referrerClickTimestampServerSeconds", ReferrerClickTimestampServerSeconds}
            };
        }
    }
}