using Shared.Core.IoC;
using Shared.Core.Repository.IntType;

namespace Shared.Repository.Ump
{
    [Repository]
#if UNITY_ANDROID && !UNITY_EDITOR
    public class GdprAppliesRepository : IntDefaultSharedPreferencesRepository
#else
    public class GdprAppliesRepository : IntPlayerPrefsRepository
#endif
    {
        public GdprAppliesRepository() : base("IABTCF_gdprApplies", 0)
        {
        }
    }
}