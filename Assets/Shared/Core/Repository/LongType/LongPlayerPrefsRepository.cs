using UnityEngine;

namespace Shared.Core.Repository.LongType
{
    public class LongPlayerPrefsRepository : LongBaseRepository
    {
        private const string Tag = "LongPlayerPrefsRepository";

        public LongPlayerPrefsRepository(string name = null, long defaultValue = 0) : base(name, defaultValue)
        {
        }

        public override long Get() => _GetFromPlayerPref();

        public override void Set(long newValue)
        {
            var oldValue = Get();
            if (newValue == oldValue) return;
            _SaveToPlayerPref(newValue);
            
            onValueUpdated.Invoke(oldValue, newValue);
            RepositoryEvents.OnLongValueChangedEvent.Invoke(this, oldValue, newValue);
        }

        public override void Delete()
        {
            PlayerPrefs.DeleteKey(Name);
            PlayerPrefs.Save();
        }
        
        // -------------------------------------------------------------------------------------------------------------
        // Inner Functions
        // -------------------------------------------------------------------------------------------------------------
        private long _GetFromPlayerPref()
        {
            var str = PlayerPrefs.GetString(Name, null);
            return string.IsNullOrEmpty(str) ? DefaultValue : long.Parse(str);
        }

        private void _SaveToPlayerPref(long val)
        {
            PlayerPrefs.SetString(Name, "" + val);
            PlayerPrefs.Save();
        }
        //------------------------------------------------------------
    }
}
