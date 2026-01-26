#if FACEBOOK_INSTANT
using Shared.Common;

namespace Shared.PlayerPrefsRepository.FacebookInstant.Internal
{
    public interface IFacebookInstantDataController
    {
        IFetchFacebookInstantDataOperation Fetch(params string[] keys);
        void SetBool(string key, bool boolValue);
        bool GetBool(string key, bool defaultValue = false);
        
        void SetString(string key, string stringValue);
        string GetString(string key, string defaultValue);
        
        void SetInt(string key, int intValue);
        int GetInt(string key, int defaultValue);

        bool HasKey(string key);
        bool DeleteKey(string key);

        void Save();

        IAsyncOperation Fetch();
    }
}
#endif