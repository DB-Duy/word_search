using Newtonsoft.Json;

namespace Shared.Entity.Session
{
    [System.Serializable]
    public class SessionEntity
    {
        [JsonProperty("session_id")] public long SessionId { get; set; }
        [JsonProperty("session_count")] public int SessionCount  { get; set; }
        /// <summary>
        /// Xác định session này từ onCreated or onResumed.
        /// </summary>
        [JsonProperty("is_from_resume")] public bool IsFromResume  { get; set; }
        [JsonProperty("native_class")] public NativeClassEntity NativeClass { get; set; }
        

        public static SessionEntity NewInstance(long sessionId, int sessionCount, bool isFromResume, string activity = null)
        {
            return new SessionEntity
            {
                SessionId = sessionId,
                SessionCount = sessionCount,
                IsFromResume = isFromResume,
                NativeClass = new NativeClassEntity(activity)
            };
        }
    }
}