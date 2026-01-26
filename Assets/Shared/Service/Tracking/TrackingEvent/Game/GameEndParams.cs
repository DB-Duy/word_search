using System.Collections.Generic;
using Newtonsoft.Json;
using Shared.Service.Tracking.Common;
using Shared.Tracking.Templates;
using Shared.Utils;

namespace Shared.Tracking.Models.Game
{
    public class GameEndParams : BaseTrackingEvent, IConvertableEvent
    {
        [JsonIgnore] public override string EventName => "game_end";

        [JsonProperty("win")] public bool Win { get; private set; }
        [JsonProperty("reason_to_die")] public ReasonToDie ReasonToDie { get; private set; }
        [JsonProperty("total_play_time")] public int TotalPlayTime { get; private set; }
        [JsonProperty("interupted_reason")] public CrashReason InterruptedReason { get; set; } = CrashReason.None;
        
        [JsonIgnore] private Dictionary<string, object> _params;
        [JsonIgnore] private Dictionary<string, object> _gameSpecificParams = new();
        
        public static GameEndParams Success(int totalPlayTime, CrashReason crashReason, Dictionary<string, object> gameSpecificParams)
        {
            return new GameEndParams
            {
                Win = true,
                ReasonToDie = ReasonToDie.NoReason,
                TotalPlayTime = totalPlayTime,
                InterruptedReason = crashReason,
                _gameSpecificParams = gameSpecificParams
            };
        }

        public static GameEndParams Fail(ReasonToDie reasonToDie, int totalPlayTime, CrashReason crashReason, Dictionary<string, object> gameSpecificParams)
        {
            return new GameEndParams
            {
                Win = false,
                ReasonToDie = reasonToDie,
                TotalPlayTime = totalPlayTime,
                InterruptedReason = crashReason,
                _gameSpecificParams = gameSpecificParams
            };
        }
        
        public override string ToString() => JsonConvert.SerializeObject(ToConvertableEvent());

        public Dictionary<string, object> ToConvertableEvent()
        {
            if (_params != null) return _params;
            
            _params = new Dictionary<string, object> (ExParams)
            {
                { "win", Win },
                { "reason_to_die", ReasonToDie.Value },
                { "total_play_time", TotalPlayTime },
                { "interupted_reason", InterruptedReason.Value },
            };
            if (_gameSpecificParams != null) _params.Upsert(_gameSpecificParams);
            return _params;
        }
    }
}