using System.Collections;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json;

namespace Shared.Utils
{
    [DisallowMultipleComponent]
    public class IPLocationDetector : MonoBehaviour
    {
        [System.Serializable]
        public class Country
        {
            [JsonProperty("businessName")] private string _businessName;
            [JsonProperty("businessWebsite")] private string _businessWebsite;
            [JsonProperty("city")] private string _city;
            [JsonProperty("continent")] private string _continent;
            [JsonProperty("country")] private string _country;
            [JsonProperty("countryCode")] private string _countryCode;
            [JsonProperty("ipName")] private string _ipName;
            [JsonProperty("ipType")] private string _ipType;
            [JsonProperty("isp")] private string _isp;
            [JsonProperty("lat")] private string _lat;
            [JsonProperty("lon")] private string _lon;
            [JsonProperty("org")] private string _org;
            [JsonProperty("query")] private string _query;
            [JsonProperty("region")] private string _region;
            [JsonProperty("status")] private string _status;

            public override string ToString() => JsonConvert.SerializeObject(this);
        }

        const string TAG = "IPLocationDetector";

        private static IPLocationDetector _instance;
        public static IPLocationDetector Instance
        {
            get
            {
                if (_instance != null) return _instance;
                var go = new GameObject(TAG);
                _instance = go.AddComponent<IPLocationDetector>();
                DontDestroyOnLoad(go);
                return _instance;
            }
        }

        private Country _country = null;

        public void DetectCountry()
        {
            SharedLogger.Log($"{TAG}->DetectCountry: APS");
            StartCoroutine(_DetectCountry());
        }

        private IEnumerator _DetectCountry()
        {
            var request = UnityWebRequest.Get("https://extreme-ip-lookup.com/json/?key=J1XZpbNPmfTaFJaDx2X9");
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                SharedLogger.Log($"{TAG}->_DetectCountry: APS SUCCESS = {request.downloadHandler.text}");
                _country = JsonConvert.DeserializeObject<Country>(request.downloadHandler.text);
            }
            else
            {
                SharedLogger.Log($"{TAG}->_DetectCountry: APS ERROR: {request.result.ToString()}");
            }
        }

        [Conditional("LOG_INFO")]
        private void OnApplicationPause(bool pauseStatus)
        {
            SharedLogger.Log($"{TAG}->OnApplicationPause: APS Detect Country = {_country}");
        }
    }
}