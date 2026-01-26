using System.Collections.Generic;
using Newtonsoft.Json;
using Shared.Service.Tracking.Common;
using Shared.Tracking.Templates;

namespace Shared.Tracking.Models.Game
{
    public class GameErrorParams : BaseTrackingEvent, IConvertableEvent
    {
        [JsonProperty("event_name")] public override string EventName => "game_error";
        
        [JsonProperty("severity")] public LogSeverity Severity { get; }
        [JsonProperty("message")] public string Message { get; }
        
        [JsonIgnore] private Dictionary<string, object> _params;
        
        public GameErrorParams(string message, LogSeverity severity)
        {
            Message = message;
            Severity = severity;
        }

        public override string ToString() => JsonConvert.SerializeObject(ToConvertableEvent());

        public Dictionary<string, object> ToConvertableEvent()
        {
            return _params ??= new Dictionary<string, object> (ExParams)
            {
                { "severity", Severity.Value },
                { "message", Message }
            };
        }
    }
}