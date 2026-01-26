#if UNITY_ANDROID
using Shared.Utils;
using UnityEngine;

namespace Shared.Core.Repository.IntType
{
    public class IntDefaultSharedPreferencesRepository : IIntRepository, ISharedUtility
    {
        private const string Tag = "IntDefaultSharedPreferencesRepository";
        
        public string Name { get; }
        public int DefaultValue { get; }

        public IIntRepository.OnIntValueUpdated onIntValueUpdated { get; } = new();
        
        private readonly AndroidJavaObject _sharedPreferences;

        public IntDefaultSharedPreferencesRepository(string name = null, int defaultValue = 0)
        {
            Name = name;
            DefaultValue = defaultValue;
            _sharedPreferences = this.GetDefaultSharedPreferences();
        }

        public int Get()
        {
            var v = _sharedPreferences.Call<int>("getInt", Name, DefaultValue);
            SharedLogger.LogJson($"{Tag}->GetInt", nameof(Name), Name, nameof(v), v);
            return v;
        }

        public void Set(int newValue)
        {
            throw new System.NotImplementedException();
        }

        public bool SetIfLargeThanCurrentValue(int newValue)
        {
            throw new System.NotImplementedException();
        }

        public int AddMore(int more)
        {
            throw new System.NotImplementedException();
        }

        public int Minus(int less)
        {
            throw new System.NotImplementedException();
        }

        public bool IsGreaterThanEqual(int val)
        {
            throw new System.NotImplementedException();
        }

        public bool IsGreaterThanZero()
        {
            throw new System.NotImplementedException();
        }

        public void Delete()
        {
            throw new System.NotImplementedException();
        }
    }
}
#endif