
using Shared.Core.IoC;
using Shared.Core.Repository.JsonType;
using Entity.UserData;
using Utils;

namespace Repository.UserData
{
    [Repository]
    public class UserDataRepository : CachedJsonPlayerPrefsRepository<UserDataEntity>
    {
    }
}