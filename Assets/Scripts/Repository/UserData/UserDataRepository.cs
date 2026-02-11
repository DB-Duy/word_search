
using Shared.Core.IoC;
using Shared.Core.Repository.JsonType;
using Entity.UserData;

namespace Repository.UserData
{
    [Repository]
    public class UserDataRepository : JsonPlayerPrefsRepository<UserDataEntity>
    {
    }
}