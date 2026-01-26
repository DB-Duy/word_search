#if FACEBOOK_INSTANT
using Shared.PlayerPrefsRepository.FacebookInstant.Internal;
using Shared.Utils;

namespace Shared.PlayerPrefsRepository.FacebookInstant
{
    public class BoolFacebookInstantRepositoryImpl : IBoolRepository
    {
        // ReSharper disable once InconsistentNaming
        private const string TAG = "BoolFacebookInstantRepositoryImpl";
        
        public IBoolRepository.OnValueChanged onValueChanged { get; } = new();
        
        public string Name { get; }
        private readonly bool _defaultValue;
        
        private IFacebookInstantDataController PlayerPrefs => FacebookInstantDataController.Instance;
        
        public BoolFacebookInstantRepositoryImpl(string name, bool defaultValue = false)
        {
            Name = name;
            _defaultValue = defaultValue;
        }

        // ------------------------------------------------------
        private bool _GetFromPlayerPref() => PlayerPrefs.GetBool(Name, _defaultValue);

        private void _SaveToPlayerPref(bool val)
        {
            SharedLogger.Log($"{TAG}->_SaveToPlayerPref: {Name} {val}");
            PlayerPrefs.SetBool(Name, val);
            PlayerPrefs.Save();
        }
        // ------------------------------------------------------

        public bool Get() => _GetFromPlayerPref();

        public void Set(bool val)
        {
            var oldOne = Get();
            if (val != oldOne)
            {
                SharedLogger.Log($"{TAG}->Set: {Name}= {val}");
                _SaveToPlayerPref(val);
                onValueChanged.Invoke(val);
            }
        }
    }
}
#endif