using Newtonsoft.Json;
using UnityEngine;

namespace Shared.Core.Repository.JsonType
{
    public class JsonPlayerPrefsRepository<T> : JsonBaseRepository<T>
    {
        private const string Tag = "JsonPlayerPrefsRepository";

        public JsonPlayerPrefsRepository(string name = null, T defaultValue = default) : base(name, defaultValue)
        {
        }
        
        public override T Get()
        {
            var str = _GetFromPlayerPref();
            return string.IsNullOrEmpty(str) ? DefaultValue : JsonConvert.DeserializeObject<T>(str);
        }

        public override void Save(object ob)
        {
            var olaValue = _GetFromPlayerPref();
            var newValue = JsonConvert.SerializeObject(ob);
            if (newValue == olaValue) return;
            _SaveToPlayerPref(newValue);
            onValueUpdated?.Invoke((T)ob);
        }

        public override bool IsExisted()
        {
            var str = _GetFromPlayerPref();
            return !string.IsNullOrEmpty(str);
        }

        public override void Delete()
        {
            PlayerPrefs.DeleteKey(Name);
            PlayerPrefs.Save();
        }    
        
        // -------------------------------------------------------------------------------------------------------------
        // Inner functions
        // -------------------------------------------------------------------------------------------------------------
        private string _GetFromPlayerPref()
        {
            return PlayerPrefs.GetString(Name, null);
        }

        private void _SaveToPlayerPref(string val)
        {
            PlayerPrefs.SetString(Name, val);
            PlayerPrefs.Save();
        }

        
    }
}
