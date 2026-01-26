#if FACEBOOK_INSTANT
using Shared.PlayerPrefsRepository.FacebookInstant.Internal;
using Shared.Utils;

namespace Shared.PlayerPrefsRepository.FacebookInstant
{
    public class StringFacebookInstantRepositoryImpl : IStringRepository
    {
        // ReSharper disable once InconsistentNaming
        private const string TAG = "StringFacebookInstantRepositoryImpl";

        public IStringRepository.OnValueUpdated onValueUpdated { get; } = new();

        public string Name { get; }
        private readonly string _defaultValue;

        private IFacebookInstantDataController PlayerPrefs => FacebookInstantDataController.Instance;

        public StringFacebookInstantRepositoryImpl(string name, string defaultValue = "")
        {
            Name = name;
            _defaultValue = defaultValue;
        }

        // -------------------------------------------------
        private string GetFromPlayerPref() => PlayerPrefs.GetString(Name, _defaultValue);

        private void SaveToPlayerPref(string val)
        {
            SharedLogger.Log($"{TAG}->_SaveToPlayerPref: {Name} {val}");
            PlayerPrefs.SetString(Name, val);
            PlayerPrefs.Save();
        }

        // -------------------------------------------------
        public bool InitIfNotExisted(string value)
        {
            if (PlayerPrefs.HasKey(Name)) return false;
            SaveToPlayerPref(value);
            return true;
        }

        public string Get() => GetFromPlayerPref();

        public bool Set(string newValue)
        {
            var oldValue = Get();
            if (newValue.Equals(oldValue)) return false;

            SaveToPlayerPref(newValue);
            onValueUpdated.Invoke(oldValue, newValue);
            return true;
        }

        public void Delete()
        {
            PlayerPrefs.DeleteKey(Name);
            PlayerPrefs.Save();
        }
    }
}
#endif