using System;
using Repository.Puzzle;
using Shared.Core.IoC;
using UnityEngine.AddressableAssets;
using Zenject;

namespace Service.Puzzle
{
    [Service]
    public class PuzzleService : IInitializable
    {
        private const string Tag = "PuzzleService";
        private const string PuzzlesDataAddressablePath = "PuzzlesDataStore";

        [Inject] private PuzzleRepository _puzzleRepository;

        private PuzzlesDataStore _puzzlesDataStore;
        public bool IsInitialized;
        private bool _isLevelDataLoaded;

        public void Initialize()
        {
            var handle = Addressables.LoadAssetAsync<PuzzlesDataStore>(PuzzlesDataAddressablePath);
            handle.Completed += operation =>
            {
                if (operation.Status == UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationStatus.Succeeded)
                {
                    _puzzlesDataStore = operation.Result;
                    _isLevelDataLoaded = true;
                    IsInitialized = true;
                }
            };
        }

        public GamePuzzle GetPuzzleById(int puzzleId)
        {
            return _puzzlesDataStore.GetPuzzleData(puzzleId);
        }
    }
}