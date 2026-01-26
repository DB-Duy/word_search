using UnityEngine;

namespace Shared.Core.Repository.IntType
{
    public class IntPlayerPrefsRepository : IntBaseRepository
    {
        private const string Tag = "IntPlayerPrefsRepository";
        
        public IntPlayerPrefsRepository(string name = null, int defaultValue = 0) : base(name, defaultValue)
        {
        }

        public override int Get() => _GetFromPlayerPref();

        public override void Set(int newValue)
        {
            var oldValue = Get();
            if (oldValue == newValue) return;
            _SaveToPlayerPref(newValue);
            onIntValueUpdated?.Invoke(oldValue, newValue);
            RepositoryEvents.OnIntValueChangedEvent.Invoke(this, oldValue, newValue);
        }

        public override void Delete()
        {
            PlayerPrefs.DeleteKey(Name);
            PlayerPrefs.Save();
        }
        
        // -------------------------------------------------------------------------------------------------------------
        // Inner Functions
        // -------------------------------------------------------------------------------------------------------------
        private int _GetFromPlayerPref() => PlayerPrefs.GetInt(Name, DefaultValue);

        private void _SaveToPlayerPref(int val)
        {
            PlayerPrefs.SetInt(Name, val);
            PlayerPrefs.Save();
        }
    }
}
