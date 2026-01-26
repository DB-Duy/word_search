using Shared.Core.IoC;
using Shared.Core.Repository.JsonType;
using Shared.Entity.InAppUpdate;

namespace Shared.Repository.InAppUpdate
{
    [Repository]
    public class InAppUpdateRepository : JsonStoreRepository<InAppUpdateEntity>
    {
    }
}