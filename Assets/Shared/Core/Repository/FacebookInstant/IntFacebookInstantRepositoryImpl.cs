#if FACEBOOK_INSTANT
using Shared.PlayerPrefsRepository.FacebookInstant.Internal;
using Shared.Utils;

namespace Shared.PlayerPrefsRepository.FacebookInstant
{
    public class IntFacebookInstantRepositoryImpl : IIntRepository
    {
        // ReSharper disable once InconsistentNaming
        private const string TAG = "IntFacebookInstantRepositoryImpl";

        public IIntRepository.OnIntValueUpdated onIntValueUpdated { get; } = new();
        public IIntRepository.OnIntValueDecreased onIntValueDecreased { get; } = new();
        public IIntRepository.OnIntValueIncreased onIntValueIncreased { get; } = new();

        public string Name { get; }
        private readonly int _defaultValue;

        private IFacebookInstantDataController PlayerPrefs => FacebookInstantDataController.Instance;

        public IntFacebookInstantRepositoryImpl(string name, int defaultValue = 0)
        {
            Name = name;
            _defaultValue = defaultValue;
        }

        //------------------------------------------------------------
        private int _GetFromPlayerPref() => PlayerPrefs.GetInt(this.Name, _defaultValue);

        private void _SaveToPlayerPref(int val)
        {
            SharedLogger.Log($"{TAG}->_SaveToPlayerPref: {Name} {val}");
            PlayerPrefs.SetInt(Name, val);
            PlayerPrefs.Save();
        }

        //------------------------------------------------------------
        public bool InitIfNotExisted(int value)
        {
            if (PlayerPrefs.HasKey(Name)) return false;
            _SaveToPlayerPref(value);
            return true;
        }

        public int Get() => _GetFromPlayerPref();

        public void Set(int value)
        {
            var oldValue = Get();
            if (value != oldValue) _SaveToPlayerPref(val: value);
        }

        public bool SetIfLargeThanCurrentValue(int newValue)
        {
            var oldValue = Get();
            if (newValue <= oldValue) return false;
            _SaveToPlayerPref(newValue);
            onIntValueUpdated?.Invoke(oldValue, newValue);
            onIntValueIncreased?.Invoke(oldValue, newValue);
            return true;
        }

        public int AddMore(int more)
        {
            var oldValue = _GetFromPlayerPref();
            var newValue = oldValue + more;
            _SaveToPlayerPref(newValue);

            onIntValueUpdated?.Invoke(oldValue, newValue);
            onIntValueIncreased?.Invoke(oldValue, newValue);

            return newValue;
        }

        public int Minus(int less)
        {
            var oldValue = _GetFromPlayerPref();
            var newValue = oldValue - less;
            _SaveToPlayerPref(newValue);

            onIntValueUpdated?.Invoke(oldValue, newValue);
            onIntValueDecreased?.Invoke(oldValue, newValue);
            return newValue;
        }

        public bool IsGreaterThanEqual(int val)
        {
            var total = _GetFromPlayerPref();
            return total >= val;
        }

        public bool IsGreaterThanZero()
        {
            var total = _GetFromPlayerPref();
            return total > 0;
        }

        public void Delete()
        {
            PlayerPrefs.DeleteKey(Name);
            PlayerPrefs.Save();
        }
    }
}
#endif