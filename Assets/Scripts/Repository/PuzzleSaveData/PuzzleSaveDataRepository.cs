
using Shared.Core.IoC;
using Shared.Core.Repository.JsonType;
using Entity.PuzzleSaveData;

namespace Repository.PuzzleSaveData
{
    [Repository]
    public class PuzzleSaveDataRepository : JsonPlayerPrefsRepository<PuzzleSaveDataEntity>
    {
    }
}