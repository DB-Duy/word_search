using System.Collections.Generic;
using Newtonsoft.Json;
using Shared.Tracking.Templates;

namespace Shared.Tracking.Models.Session
{
    [System.Serializable]
    public class SessionResumeEvent : BaseTrackingEvent, IConvertableEvent
    {
        [JsonProperty("event_name")] public override string EventName => "session_resume_customs";
        
        [JsonProperty("session_id")] public long SessionId { get; private set; }
        [JsonProperty("class_name")] public string ClassName { get; private set; }
        [JsonProperty("package_name")] public string PackageName { get; private set; }
        
        [JsonIgnore] private Dictionary<string, object> _params;

        public SessionResumeEvent(long sessionId, string className = null, string packageName = null)
        {
            SessionId = sessionId;
            ClassName = className;
            PackageName = packageName;
        }

        public override string ToString() => JsonConvert.SerializeObject(ToConvertableEvent());
        
        public Dictionary<string, object> ToConvertableEvent()
        {
            if (_params != null) return _params;
            _params = new Dictionary<string, object>(ExParams);
            _params["session_id"] = SessionId;
            _params["class_name"] = ClassName;
            _params["package_name"] = PackageName;
            return _params;
        }
    }
}