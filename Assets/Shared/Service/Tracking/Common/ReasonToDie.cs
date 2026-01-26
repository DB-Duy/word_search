using Newtonsoft.Json;

namespace Shared.Service.Tracking.Common
{
    [System.Serializable]
    public class ReasonToDie
    {
        [JsonProperty("value")] private string _v;
        [JsonIgnore] public string Value => _v;
        public ReasonToDie(string v) => _v = v;

        public static readonly ReasonToDie GameInterrupted = new("game_interrupted");
        public static readonly ReasonToDie Unknown = new("unknown");
        public static readonly ReasonToDie NoReason = new("null");
    }
}