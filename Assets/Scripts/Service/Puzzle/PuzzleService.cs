using System;
using Entity.PuzzleSaveData;
using Repository.PuzzleSaveData;
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

        [Inject] private PuzzleSaveDataRepository _puzzleRepository;

        public bool HasSaveData
        {
            get
            {
                var entity = _puzzleRepository.Get();
                return entity.SolvedWords.Count > 0;
            }
        }

        private PuzzlesDataStore _puzzlesDataStore;
        public bool IsInitialized;
        private bool _isLevelDataLoaded;

        public PuzzleSaveDataEntity SaveData => _puzzleRepository.Get();

        public void Initialize()
        {
            var handle = Addressables.LoadAssetAsync<PuzzlesDataStore>(PuzzlesDataAddressablePath);
            var save = SaveData;
            if (save.SolvedWords == null)
            {
                save.SolvedWords = new();
                SavePuzzleData(save);
            }

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

        public void SavePuzzleData(PuzzleSaveDataEntity data)
        {
            _puzzleRepository.Save(data);
        }

        public GamePuzzle GetPuzzleById(int puzzleId)
        {
            return _puzzlesDataStore.GetPuzzleData(puzzleId);
        }

        public void ClearPuzzleSaveData()
        {
            var save = SaveData;
            save.SolvedWords.Clear();
            _puzzleRepository.Save(save);
        }
    }
}