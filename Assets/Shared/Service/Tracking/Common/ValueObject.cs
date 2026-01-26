using Newtonsoft.Json;

namespace Shared.Service.Tracking.Common
{
    [System.Serializable]
    public abstract class ValueObject
    {
        [JsonProperty("value")] protected string _v;
        [JsonIgnore] public string Value => _v;
        protected ValueObject(string v) => _v = v;
        
        public override string ToString() => JsonConvert.SerializeObject(this);
    }
}