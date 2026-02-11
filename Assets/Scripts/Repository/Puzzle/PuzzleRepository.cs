
using Shared.Core.IoC;
using Shared.Core.Repository.JsonType;
using Entity.Puzzle;

namespace Repository.Puzzle
{
    [Repository]
    public class PuzzleRepository : JsonPlayerPrefsRepository<PuzzleEntity>
    {
    }
}