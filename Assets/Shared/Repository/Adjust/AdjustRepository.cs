#if ADJUST
using Shared.Core.IoC;
using Shared.Core.Repository.JsonType;
using Shared.Entity.SharedAdjust;

namespace Shared.Repository.Adjust
{
    [Repository]
    public class AdjustRepository : JsonStoreRepository<AdjustEntity>
    {
    }
}
#endif