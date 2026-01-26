using System;
using Newtonsoft.Json;

namespace Shared.Entity.Mmp
{
    [Serializable]
    public class MmpConfig
    {
        [Serializable]
        public class EventConfig
        {
            [JsonProperty("eventName")] public string EventName { get; private set; }
            [JsonProperty("paramCondition")] public string ParamCondition { get; private set; }
            [JsonProperty("appsflyerEventName")] public string appsflyerEventName;
            [JsonProperty("adjustEventToken")] public string adjustEventToken;
        }
    }
}