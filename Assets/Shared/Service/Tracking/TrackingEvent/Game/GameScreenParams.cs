using System.Collections.Generic;
using Newtonsoft.Json;
using Shared.Service.Tracking.Common;
using Shared.Tracking.Templates;

namespace Shared.Tracking.Models.Game
{
    public class GameScreenParams : BaseTrackingEvent, IConvertableEvent, ICountEvent
    {
        [JsonProperty("event_name")] public override string EventName => "game_screen";
        [JsonProperty("count_key")] public string CountKey { get; }

        [JsonProperty("screen")] public TrackingScreen Screen { get; }
        [JsonProperty("count")] public long Count { get; private set; }

        [JsonIgnore] private Dictionary<string, object> _params;

        public GameScreenParams(TrackingScreen screen)
        {
            Screen = screen;
            CountKey = $"scene_count_{screen.Value}";
        }

        public void SetEventCount(long count) => Count = count;

        public override string ToString() => JsonConvert.SerializeObject(ToConvertableEvent());
        public Dictionary<string, object> ToConvertableEvent()
        {
            return _params ??= new Dictionary<string, object>(ExParams)
            {
                { "screen", Screen.Value },
                { "count", Count }
            };
        }
    }
}