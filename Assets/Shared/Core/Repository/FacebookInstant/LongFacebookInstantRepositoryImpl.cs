#if FACEBOOK_INSTANT
using Shared.PlayerPrefsRepository.FacebookInstant.Internal;
using Shared.Utils;

namespace Shared.PlayerPrefsRepository.FacebookInstant
{
    public class LongFacebookInstantRepositoryImpl : ILongRepository
    {
        // ReSharper disable once InconsistentNaming
        private const string TAG = "LongFacebookInstantRepositoryImpl";

        public ILongRepository.OnValueUpdated onValueUpdated { get; } = new();
        public ILongRepository.OnValueDecreased onValueDecreased { get; } = new();
        public ILongRepository.OnValueIncreased onValueIncreased { get; } = new();

        public string Name { get; }
        private readonly long _defaultValue;

        private IFacebookInstantDataController PlayerPrefs => FacebookInstantDataController.Instance;

        public LongFacebookInstantRepositoryImpl(string name, long defaultValue = 0)
        {
            Name = name;
            _defaultValue = defaultValue;
        }

        //------------------------------------------------------------
        private long _GetFromPlayerPref()
        {
            var str = PlayerPrefs.GetString(Name, null);
            return string.IsNullOrEmpty(str) ? _defaultValue : long.Parse(str);
        }

        private void _SaveToPlayerPref(long val)
        {
            SharedLogger.Log($"{TAG}->_SaveToPlayerPref: {Name} {val}");
            PlayerPrefs.SetString(Name, "" + val);
            PlayerPrefs.Save();
        }

        //------------------------------------------------------------
        public bool InitIfNotExisted(long value)
        {
            if (PlayerPrefs.HasKey(Name)) return false;
            _SaveToPlayerPref(value);
            return true;
        }

        public long Get() => _GetFromPlayerPref();

        public void Set(long value) => _SaveToPlayerPref(val: value);

        public bool SetIfLargeThanCurrentValue(long newValue)
        {
            var oldValue = Get();
            if (newValue <= oldValue) return false;
            _SaveToPlayerPref(newValue);
            return true;
        }

        public long AddMore(long more)
        {
            var oldValue = _GetFromPlayerPref();
            var newValue = oldValue + more;
            _SaveToPlayerPref(newValue);

            onValueUpdated?.Invoke(oldValue, newValue);
            onValueIncreased?.Invoke(oldValue, newValue);

            return newValue;
        }

        public long Minus(long less)
        {
            var oldValue = _GetFromPlayerPref();
            var newValue = oldValue - less;
            _SaveToPlayerPref(newValue);

            onValueUpdated?.Invoke(oldValue, newValue);
            onValueDecreased?.Invoke(oldValue, newValue);

            return newValue;
        }

        public bool IsGreaterThanEqual(long val)
        {
            var total = _GetFromPlayerPref();
            return total >= val;
        }

        public void Delete()
        {
            PlayerPrefs.DeleteKey(Name);
            PlayerPrefs.Save();
        }
    }
}
#endif