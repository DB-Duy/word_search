
using Repository.PuzzleSaveData;
using Shared.Core.IoC;
using Zenject;

namespace Service.PuzzleSaveData
{
    [Service]
    public class PuzzleSaveDataService
    {
        private const string Tag = "PuzzleSaveDataService";
        
        [Inject] private PuzzleSaveDataRepository _puzzleSaveDataRepository;
        
    }
}