#if UNITY_ANDROID
using Shared.Utils;
using UnityEngine;

namespace Shared.Core.Repository.StringType
{
    public class StringDefaultSharedPreferencesRepository : StringBaseRepository, ISharedUtility
    {
        private const string Tag = "StringDefaultSharedPreferencesRepository";
        
        private readonly AndroidJavaObject _sharedPreferences;

        public StringDefaultSharedPreferencesRepository(string name = null, string defaultValue = null) : base(name, defaultValue)
        {
            _sharedPreferences = this.GetDefaultSharedPreferences();
        }

        public override string Get()
        {
            var v = _sharedPreferences.Call<string>("getString", Name, DefaultValue); 
            SharedLogger.LogJson($"{Tag}->GetString", nameof(Name), Name, nameof(DefaultValue), DefaultValue, nameof(v), v);
            return v;
        }

        public override bool Set(string diffValue)
        {
            var editor = _sharedPreferences.Call<AndroidJavaObject>("edit");
            editor.Call<AndroidJavaObject>("putString", Name, diffValue);
            var result = editor.Call<bool>("commit");
            SharedLogger.LogJson($"{Tag}->SetString", nameof(Name), Name, nameof(DefaultValue), DefaultValue, nameof(diffValue), diffValue, nameof(result), result);
            return true;
        }

        public override void Delete()
        {
            throw new System.NotImplementedException();
        }
    }
}
#endif