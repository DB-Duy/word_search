using System.Collections.Generic;
using Newtonsoft.Json;
using Shared.Tracking.Templates;

namespace Shared.Tracking.Models.Game
{
    public class TutorialParams : BaseTrackingEvent, IConvertableEvent
    {
        [JsonProperty("event_name")] public override string EventName => "tutorial";
        
        /// <summary>
        /// "Các bước chơi tutorial
        /// -1: bắt đầu chơi tut
        /// 1,2,3: các bước chơi tut
        /// -2: complete
        /// 0: skip"
        /// </summary>
        [JsonProperty("step")] public int Step { get; }
        [JsonIgnore] private Dictionary<string, object> _params;

        public TutorialParams(int step)
        {
            Step = step;
        }

        public override string ToString() => JsonConvert.SerializeObject(ToConvertableEvent());
        
        public Dictionary<string, object> ToConvertableEvent()
        {
            return _params ??= new Dictionary<string, object> (ExParams)
            {
                { "step", Step }
            };
        }
    }
}