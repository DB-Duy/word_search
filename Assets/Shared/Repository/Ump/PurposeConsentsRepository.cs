using Shared.Core.IoC;
using Shared.Core.Repository.StringType;

namespace Shared.Repository.Ump
{
    [Repository]
#if UNITY_ANDROID && !UNITY_EDITOR
    public class PurposeConsentsRepository :StringDefaultSharedPreferencesRepository
#else
    public class PurposeConsentsRepository : StringPlayerPrefsRepository
#endif
    {
        public PurposeConsentsRepository() : base("IABTCF_PurposeConsents", string.Empty)
        {
        }
    }
}