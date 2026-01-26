using System.Collections.Generic;
using Newtonsoft.Json;
using Shared.Service.Tracking.Common;
using Shared.Tracking.Templates;
using Shared.Utils;

namespace Shared.Tracking.Models.Game
{
    public class GameInterruptedParams : ITrackingEvent, IConvertableEvent, IExParamsEvent
    {
        public string EventName => "game_interupted";

        [JsonProperty("reason")] public CrashReason CrashReason { get; }
        [JsonIgnore] private Dictionary<string, object> _params;
        [JsonIgnore] public Dictionary<string, object> ExParams { get; } = new();
        [JsonIgnore] private readonly HashSet<string> _allowExParamNames;


        public GameInterruptedParams(CrashReason crashReason, Dictionary<string, object> exParams, params string[] allowParamNames)
        {
            CrashReason = crashReason;
            ExParams.Upsert(exParams);
            _allowExParamNames = new HashSet<string>(allowParamNames);
        }
        
        public GameInterruptedParams(CrashReason crashReason, Dictionary<string, object> exParams) : this(crashReason, exParams, "event_timestamp")
        {}

        public override string ToString() => JsonConvert.SerializeObject(ToConvertableEvent());
        
        public Dictionary<string, object> ToConvertableEvent()
        {
            return _params ??= new Dictionary<string, object>(ExParams)
            {
                { "reason", CrashReason.Value }
            };
        }
        
        public void AddParams(string paramName1, object paramValue1)
        {
            if (_ValidateParamName(paramName1)) ExParams.UpsertPrimaryValue(paramName1, paramValue1);
        }
        
        private bool _ValidateParamName(string paramName) => !string.IsNullOrEmpty(paramName) && _allowExParamNames.Contains(paramName);

    }
}