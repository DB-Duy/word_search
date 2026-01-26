using System.Collections.Generic;
using Newtonsoft.Json;
using Shared.Tracking.Templates;

namespace Shared.Tracking.Models.Game
{
    public class AbTestParams : BaseTrackingEvent, IConvertableEvent
    {
        public override string EventName => "ab_testing";
        [JsonIgnore] private Dictionary<string, object> _params;
        [JsonProperty("config_name")] public string ConfigName { get; private set; }
        [JsonProperty("ab_variant")] public string AbVariant { get; private set; }

        public AbTestParams(string configName, string abVariant)
        {
            ConfigName = configName;
            AbVariant = abVariant;
        }

        public override string ToString() => JsonConvert.SerializeObject(ToConvertableEvent());

        public Dictionary<string, object> ToConvertableEvent()
        {
            return _params ??= new Dictionary<string, object>(ExParams)
            {
                { "config_name", ConfigName },
                { "ab_variant", AbVariant }
            };
        }
    }
}