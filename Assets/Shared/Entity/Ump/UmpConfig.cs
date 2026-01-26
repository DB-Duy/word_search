using Newtonsoft.Json;

namespace Shared.Entity.Ump
{
    [System.Serializable]
    public class UmpConfig
    {
        [JsonProperty("enable_consent_android")] public bool IsAndroidEnable { get; private set; }
        [JsonProperty("enable_consent_iOS")] public bool IsIosEnable { get; private set; }

        [JsonIgnore]
        public bool IsEnable
        {
            get
            {
#if UNITY_ANDROID
                return IsAndroidEnable;
#elif UNITY_IOS
                return IsIosEnable;
#else
                return false;       
#endif
            }
        }


    }
}