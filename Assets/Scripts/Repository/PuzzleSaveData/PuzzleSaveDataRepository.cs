
using Shared.Core.IoC;
using Shared.Core.Repository.JsonType;
using Entity.PuzzleSaveData;
using Utils;

namespace Repository.PuzzleSaveData
{
    [Repository]
    public class PuzzleSaveDataRepository : CachedJsonPlayerPrefsRepository<PuzzleSaveDataEntity>
    {
    }
}