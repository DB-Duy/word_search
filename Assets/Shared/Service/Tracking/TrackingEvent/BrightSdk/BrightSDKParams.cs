using System.Collections.Generic;
using Newtonsoft.Json;
using Shared.Service.Tracking.Common;
using Shared.Tracking.Templates;

namespace Shared.Tracking.Models.BrightSdk
{
    public class BrightSDKParams : BaseTrackingEvent, IConvertableEvent
    {
        public override string EventName => "bright_sdk";

        [JsonProperty("screen")] public BrightSDKScreen Screen { get; }
        [JsonProperty("action")] public BrightSDKAction Action { get; }
        
        [JsonIgnore] private Dictionary<string, object> _params;
        
        public BrightSDKParams(BrightSDKScreen screen, BrightSDKAction action)
        {
            Screen = screen;
            Action = action;
        }
        
        public override string ToString() => JsonConvert.SerializeObject(ToConvertableEvent());
        
        public Dictionary<string, object> ToConvertableEvent()
        {
            return _params ??= new Dictionary<string, object> (ExParams)
            {
                { "screen", Screen.Value },
                { "action", Action.Value }
            };
        }
    }
}