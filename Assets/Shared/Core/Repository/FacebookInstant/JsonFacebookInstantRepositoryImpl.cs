#if FACEBOOK_INSTANT
using Newtonsoft.Json;
using Shared.PlayerPrefsRepository.FacebookInstant.Internal;
using Shared.Utils;

namespace Shared.PlayerPrefsRepository.FacebookInstant
{
    public class JsonFacebookInstantRepositoryImpl<T> : IJsonRepository<T>
    {
        // ReSharper disable once InconsistentNaming
        private const string TAG = "JsonFacebookInstantRepositoryImpl";

        private string Name { get; }
        private readonly T _defaultObj;

        private IFacebookInstantDataController PlayerPrefs => FacebookInstantDataController.Instance;

        public JsonFacebookInstantRepositoryImpl(string name, T defaultObj)
        {
            Name = name;
            _defaultObj = defaultObj;
        }

        // -----------------------------------------------
        private string _GetFromPlayerPref()
        {
            return PlayerPrefs.GetString(Name, null);
        }

        private void _SaveToPlayerPref(string val)
        {
            SharedLogger.Log($"{TAG}->_SaveToPlayerPref: {Name} {val}");
            PlayerPrefs.SetString(Name, val);
            PlayerPrefs.Save();
        }
        // -----------------------------------------------

        public bool InitIfNotExisted(object value)
        {
            if (PlayerPrefs.HasKey(Name)) return false;
            Save(value);
            return true;
        }

        public T Get()
        {
            var str = _GetFromPlayerPref();
            return string.IsNullOrEmpty(str) ? _defaultObj : JsonConvert.DeserializeObject<T>(str);
        }

        public void Save(object ob)
        {
            var str = JsonConvert.SerializeObject(ob);
            _SaveToPlayerPref(str);
        }

        public bool IsExisted() => PlayerPrefs.HasKey(Name);

        public void Delete()
        {
            PlayerPrefs.DeleteKey(Name);
            PlayerPrefs.Save();
        }
    }
}
#endif