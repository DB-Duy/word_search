using Newtonsoft.Json;
using Shared.Core.Repository.JsonType;
using UnityEngine;

namespace Utils
{
    public class CachedJsonPlayerPrefsRepository<T> : JsonPlayerPrefsRepository<T>
    {
        protected T CachedValue;

        public override void Delete()
        {
            base.Delete();
            CachedValue = default;
        }   
        
        public override T Get()
        {
            if (CachedValue == null) CachedValue = base.Get();
            return CachedValue;
        }

        public override void Save(object ob)
        {
            var olaValue = _GetFromPlayerPref();
            var newValue = JsonConvert.SerializeObject(ob);
            if (newValue == olaValue) return;
            CachedValue = (T)ob;
            _SaveToPlayerPref(newValue);
            onValueUpdated?.Invoke((T)ob);
        }
        
        protected string _GetFromPlayerPref()
        {
            return PlayerPrefs.GetString(Name, null);
        }

        protected void _SaveToPlayerPref(string val)
        {
            PlayerPrefs.SetString(Name, val);
            PlayerPrefs.Save();
        }
    }
}