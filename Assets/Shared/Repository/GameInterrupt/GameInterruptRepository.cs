using System.Collections.Generic;
using Shared.Core.IoC;
using Shared.Core.Repository.JsonType;

namespace Shared.Repository.GameInterrupt
{
    [Repository]
    public class GameInterruptRepository : JsonStoreRepository<Dictionary<string, object>>
    {
    }
}