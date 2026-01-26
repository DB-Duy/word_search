using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Shared.Tracking.Templates;
using Shared.Utils;

namespace Shared.Service.Tracking.TrackingEvent.Game
{
    public class LoadingToHomeParams : ITrackingEvent, IConvertableEvent, IExParamsEvent
    {
        public string EventName => "loading_to_home";
        
        /// <summary>
        /// Time in second from Loading to Home screen
        /// </summary>
        [JsonProperty("time")] public float TimeCost { get; }
        
        [JsonIgnore] private Dictionary<string, object> _params;
        [JsonIgnore] public Dictionary<string, object> ExParams { get; } = new();
        [JsonIgnore] private readonly HashSet<string> _allowExParamNames = new() {"session_id", "event_timestamp", "level_design"};

        public LoadingToHomeParams(float timeCostInSeconds, Dictionary<string, float> serviceLoadTime)
        {
            TimeCost = timeCostInSeconds;
            var sortedList = serviceLoadTime
                .OrderByDescending(kv => kv.Value) // Sort by value (float) in descending order
                .Take(15)                         // Take the top 15 elements
                .ToList();
            foreach (var i in sortedList)
                ExParams.Add(i.Key, i.Value);
        }
        
        public override string ToString() => JsonConvert.SerializeObject(ToConvertableEvent());

        public void AddParams(string paramName1, object paramValue1)
        {
            if (_ValidateParamName(paramName1)) ExParams.UpsertPrimaryValue(paramName1, paramValue1);
        }

        private bool _ValidateParamName(string paramName) => !string.IsNullOrEmpty(paramName) && _allowExParamNames.Contains(paramName);

        public Dictionary<string, object> ToConvertableEvent()
        {
            return _params ??= new Dictionary<string, object>(ExParams)
            {
                { "time", TimeCost },
            };
        }
    }
}