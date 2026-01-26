using Shared.Core.IoC;
using Shared.Core.Repository.StringType;

namespace Shared.Repository.Ump
{
    [Repository]
#if UNITY_ANDROID && !UNITY_EDITOR
    public class UsPrivacyStringRepository : StringDefaultSharedPreferencesRepository
#else
    public class UsPrivacyStringRepository : StringPlayerPrefsRepository
#endif
    {
        public UsPrivacyStringRepository() : base("IABUSPrivacy_String", string.Empty)
        {
        }
    }
}