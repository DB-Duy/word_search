using UnityEngine;

namespace Shared.Core.Repository.StringType
{
    public class StringPlayerPrefsRepository : StringBaseRepository
    {
        private const string Tag = "StringPlayerPrefsRepository";

        public StringPlayerPrefsRepository(string name = null, string defaultValue = null) : base(name, defaultValue)
        {
        }

        public override string Get() => GetFromPlayerPref();

        public override bool Set(string newValue)
        {
            var oldValue = Get();
            if (newValue == oldValue) return false;
            SaveToPlayerPref(newValue);
            
            onValueUpdated.Invoke(oldValue, newValue);
            RepositoryEvents.OnStringValueChangedEvent.Invoke(this, oldValue, newValue);
            
            return true;
        }

        public override void Delete()
        {
            PlayerPrefs.DeleteKey(Name);
            PlayerPrefs.Save();
        }
        
        // --------------------------------------------------------------------------------------------------
        // Inner functions
        // --------------------------------------------------------------------------------------------------
        private string GetFromPlayerPref() => PlayerPrefs.GetString(Name, DefaultValue);

        private void SaveToPlayerPref(string val)
        {
            PlayerPrefs.SetString(Name, val);
            PlayerPrefs.Save();
        }
    }
}
