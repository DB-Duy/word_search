using System.Collections.Generic;
using Newtonsoft.Json;
using Shared.Service.Tracking.Common;
using Shared.Tracking.Templates;

namespace Shared.Tracking.Models.Game
{
    public class GameRestartParams : BaseTrackingEvent, IConvertableEvent
    {
        [JsonProperty("event_name")] public override string EventName => "game_restart";
        
        [JsonProperty("location")] public GameRestartLocation Location { get; }
        
        [JsonIgnore] private Dictionary<string, object> _params;

        public GameRestartParams(GameRestartLocation location)
        {
            Location = location;
        }
        
        public override string ToString() => JsonConvert.SerializeObject(ToConvertableEvent());

        public Dictionary<string, object> ToConvertableEvent()
        {
            return _params ??= new Dictionary<string, object> (ExParams)
            {
                { "location", Location.Value }
            };
        }
    }
}
