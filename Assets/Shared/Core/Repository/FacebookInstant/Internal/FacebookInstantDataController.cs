#if FACEBOOK_INSTANT
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using Newtonsoft.Json;
using Shared.Common;
using Shared.Utils;
using UnityEngine;

namespace Shared.PlayerPrefsRepository.FacebookInstant.Internal
{
    /// <summary>
    /// https://developers.facebook.com/docs/games/instant-games/sdk/fbinstant7.1
    /// </summary>
    [DisallowMultipleComponent]
    public class FacebookInstantDataController : MonoBehaviour, IFacebookInstantDataController
    {
        // ReSharper disable once InconsistentNaming
        private const string TAG = "FacebookInstantDataController";
        private const string KKK = "Key1989";

        private static FacebookInstantDataController _instance;

        public static FacebookInstantDataController Instance
        {
            get
            {
                if (_instance != null) return _instance;
                var go = new GameObject(TAG);
                _instance = go.AddComponent<FacebookInstantDataController>();
                DontDestroyOnLoad(go);
                return _instance;
            }
        }

        [DllImport("__Internal")]
        private static extern void FetchFacebookInstantKeys();
        [DllImport("__Internal")]
        private static extern void FetchFacebookInstantData(string keys);
        [DllImport("__Internal")]
        private static extern void FetchFacebookInstantSpecialData(string keys);
        [DllImport("__Internal")]
        private static extern void SaveFacebookInstantData(string jsonData);
        
        private Dictionary<string, object> _data = new();
        private IAsyncOperation _fetchOperation;

        public void Initialize()
        {
            SharedLogger.Log($"{TAG}->Initialize");
        }

        public IAsyncOperation Fetch()
        {
#if !UNITY_EDITOR
            if (_fetchOperation != null) return _fetchOperation;
            SharedLogger.Log($"{TAG}->Fetch");
            _fetchOperation = new SharedAsyncOperation();
            FetchFacebookInstantKeys();
            return _fetchOperation;
#else
            var str = PlayerPrefs.GetString("FacebookInstantData", "");
            OnDataFetchedEvent(str);
            return new SharedAsyncOperation().Success();
#endif
        }

        public void OnKeysFetchedEvent(string jsonString)
        {
            SharedLogger.Log($"{TAG}->OnKeysFetchedEvent: {jsonString}");
            var keyObject = JsonConvert.DeserializeObject<Dictionary<string, object>>(jsonString);
            if (keyObject == null) keyObject = new Dictionary<string, object>();
            if (keyObject.Count == 0)
            {
                _fetchOperation?.Success();
                return;
            }
            var keyJsonString = keyObject[KKK];
            SharedLogger.Log($"{TAG}->FetchFacebookInstantData: {keyJsonString}");
            FetchFacebookInstantData(keyJsonString.ToString());
            
        }

        public void OnDataFetchedEvent(string jsonString)
        {
            SharedLogger.Log($"{TAG}->OnDataFetchedEvent: {jsonString}");
            _data = JsonConvert.DeserializeObject<Dictionary<string, object>>(jsonString);
            if (_data == null) _data = new Dictionary<string, object>();
            _fetchOperation?.Success();
        }

        public void OnSpecialDataFetchedEvent(string dataAsString)
        {
            SharedLogger.Log($"{TAG}->OnSpecialDataFetchedEvent: {dataAsString}");
            var container = JsonConvert.DeserializeObject<Dictionary<string, string>>(dataAsString);
            var keysString = container["keys"];
            var jsonString = container["jsonString"];
            if (_fetchOperationDict.TryGetValue(keysString, out var fetch))
            {
                fetch.Success(jsonString);
                _fetchOperationDict.Remove(keysString);
            }
        }

        private readonly Dictionary<string, IFetchFacebookInstantDataOperation> _fetchOperationDict = new();
        public IFetchFacebookInstantDataOperation Fetch(params string[] keys)
        {
            var keysString = JsonConvert.SerializeObject(keys);
            SharedLogger.Log($"{TAG}->Fetch: {keysString}");
            if (_fetchOperationDict.TryGetValue(keysString, out var fetch)) return fetch;
            _fetchOperationDict.Add(keysString, new FetchFacebookInstantDataOperation());
            FetchFacebookInstantSpecialData(keysString);
            return _fetchOperationDict[keysString];
        }

        public void SetBool(string key, bool boolValue)
        {
            _data[key] = boolValue;
        }

        public bool GetBool(string key, bool defaultValue = false)
        {
            if (_data.TryGetValue(key, out var value)) return (bool)value;
            return defaultValue;
        }

        public void SetString(string key, string stringValue)
        {
            _data[key] = stringValue;
        }

        public string GetString(string key, string defaultValue)
        {
            if (_data.TryGetValue(key, out var value))
            {
                SharedLogger.Log($"{TAG}->GetString: {key} {value.GetType()}");
                if (value is Newtonsoft.Json.Linq.JObject jObject) return JsonConvert.SerializeObject(jObject);
                return (string)value;
            }
            return defaultValue;
        }

        public void SetInt(string key, int intValue)
        {
            _data[key] = intValue;
        }

        public int GetInt(string key, int defaultValue)
        {
            return _data.TryGetValue(key, out var value) ? int.Parse(value.ToString()) : defaultValue;
        }

        public bool HasKey(string key)
        {
            return _data.ContainsKey(key);
        }

        public bool DeleteKey(string key) => _data.Remove(key);

        public void Save()
        {
            var errorMessage = string.Empty;
            if (_data == null) errorMessage = "_data == null";
            else if (_data.Count == 0) errorMessage = "_data.Count == 0";

            if (!string.IsNullOrEmpty(errorMessage))
            {
                SharedLogger.Log($"{TAG}->Save: ERROR: {errorMessage}");
                return;
            }

            var keys = new HashSet<string>(_data.Keys);
            _data[KKK] = JsonConvert.SerializeObject(keys.ToArray());

            var entity = JsonConvert.SerializeObject(_data);
            SharedLogger.Log($"{TAG}->Save: {entity}");
#if !UNITY_EDITOR
            SaveFacebookInstantData(entity);
#else
            PlayerPrefs.SetString("FacebookInstantData", entity);
            PlayerPrefs.Save();
#endif
        }

        public void DeleteAll()
        {
            _data.Delete();
            var entity = JsonConvert.SerializeObject(_data);
            SharedLogger.Log($"{TAG}->Save: {entity}");
            PlayerPrefs.SetString("FacebookInstantData", entity);
            PlayerPrefs.Save();
        }
    }
}
#endif