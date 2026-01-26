using System.Collections.Generic;
using Newtonsoft.Json;
using Shared.Tracking.Templates;

namespace Shared.Tracking.Models.Game
{
    public class GameStartParams : BaseTrackingEvent, IConvertableEvent
    {
        [JsonProperty("event_name")] public override string EventName => "game_start";

        [JsonIgnore] private Dictionary<string, object> _params;

        public override string ToString() => JsonConvert.SerializeObject(ToConvertableEvent());

        public Dictionary<string, object> ToConvertableEvent()
        {
            return _params ??= new Dictionary<string, object>(ExParams);
        }
    }
}