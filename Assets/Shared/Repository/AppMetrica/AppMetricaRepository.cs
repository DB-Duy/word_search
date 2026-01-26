using Shared.Core.IoC;
using Shared.Core.Repository.JsonType;
using Shared.Entity.AppMetrica;

namespace Shared.Repository.AppMetrica
{
    [Repository]
    public class AppMetricaRepository : JsonStoreRepository<AppMetricaEntity>
    {
    }
}