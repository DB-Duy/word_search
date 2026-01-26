using System.Collections.Generic;
using Newtonsoft.Json;
using Shared.Tracking.Templates;

namespace Shared.Service.Tracking.TrackingEvent.Game
{
    public class GameTutorialParams : BaseTrackingEvent, IConvertableEvent
    {
        [JsonProperty("event_name")] public override string EventName => "tutorial";

        [JsonIgnore] private Dictionary<string, object> _params;
        
        [JsonProperty("step")] public int Step { get; private set; }

        public override string ToString() => JsonConvert.SerializeObject(ToConvertableEvent());

        public GameTutorialParams(int step)
        {
            Step = step;
        }

        public Dictionary<string, object> ToConvertableEvent()
        {
            return _params ??= new Dictionary<string, object> (ExParams)
            {
                { "step", Step }
            };
        }
    }
}