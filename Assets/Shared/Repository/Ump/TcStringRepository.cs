using Shared.Core.IoC;
using Shared.Core.Repository.StringType;

namespace Shared.Repository.Ump
{
    [Repository]
#if UNITY_ANDROID && !UNITY_EDITOR
    public class TcStringRepository : StringDefaultSharedPreferencesRepository
#else
    public class TcStringRepository : StringPlayerPrefsRepository
#endif
    {
        public TcStringRepository() : base("IABTCF_TCString", string.Empty)
        {
        }
    }
}