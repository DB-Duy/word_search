using UnityEngine;

namespace Shared.Core.Repository.BoolType
{
    public class BoolPlayerPrefsRepository : BoolBaseRepository
    {
        private const int True = 1;
        private const int False = 0;
        
        private readonly int _realDefaultValue;

        public BoolPlayerPrefsRepository(string name = null, bool defaultValue = false) : base(name, defaultValue)
        {
            _realDefaultValue = DefaultValue ? True : False;
        }
        
        public override bool Get() => _GetFromPlayerPref();
    
        public override void Set(bool newValue)
        {
            var oldValue = Get();
            if (newValue == oldValue) return;
            _SaveToPlayerPref(newValue ? True : False);
            onValueChanged.Invoke(newValue);
            foreach (var handler in Handlers) handler.OnValueChanged(newValue);
            RepositoryEvents.OnBoolValueChangedEvent.Invoke(this, newValue);
        }
        
        // -----------------------------------------------------------------------------------------------------------------
        // Inner functions
        // -----------------------------------------------------------------------------------------------------------------
        private bool _GetFromPlayerPref() => PlayerPrefs.GetInt(Name, _realDefaultValue) == True;

        private void _SaveToPlayerPref(int val)
        {
            PlayerPrefs.SetInt(Name, val);
            PlayerPrefs.Save();
        }
    }
}
