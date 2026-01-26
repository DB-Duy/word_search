using System.Collections.Generic;
using Newtonsoft.Json;
using Shared.Utils;

namespace Shared.Tracking.Templates
{
    public abstract class BaseTrackingEvent : ITrackingEvent, IExParamsEvent
    {
        [JsonIgnore] public abstract string EventName { get; }
        [JsonIgnore] public Dictionary<string, object> ExParams { get; } = new();

        public void AddParams(string paramName1, object paramValue1)
        {
            ExParams.UpsertPrimaryValue(paramName1, paramValue1);
        }
    }
}