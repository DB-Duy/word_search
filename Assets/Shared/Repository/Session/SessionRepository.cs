using Shared.Core.IoC;
using Shared.Core.Repository.JsonType;
using Shared.Entity.Session;

namespace Shared.Repository.Session
{
    [Repository]
    public class SessionRepository : JsonStoreRepository<SessionEntity>
    {
    }
}