using System.Collections;
using Service.Puzzle;
using Service.UserData;
using Shared;
using Shared.Core.IoC;
using Shared.Core.View.Scene;
using Shared.Service.SharedCoroutine;
using UnityEngine;
using View.Gameplay;
using Zenject;

namespace Service.Gameplay
{
    [Service]
    public class GameplayService : ISharedUtility, IInitializable
    {
        private const string Tag = "GameplayService";

        [Inject] private UserDataService _userDataService;
        [Inject] private PuzzleService _puzzleService;

        public void Initialize()
        {
            this.StartSharedCoroutine(StartGameCoroutine());
        }

        private IEnumerator StartGameCoroutine()
        {
            Input.multiTouchEnabled = false;
            while (!GameServicesReady())
            {
                yield return null;
            }

            UIScene.Instance.ShowScreen<GameplayScreenView>();
        }

        private bool GameServicesReady()
        {
            return _userDataService.IsInitialized && _puzzleService.IsInitialized;
        }

        public void LevelCompleted()
        {
            _userDataService.IncrementLevel();
            _puzzleService.ClearPuzzleSaveData();
            if (UIScene.Instance.GetCurrentScreen() is GameplayScreenView gameplayScreen)
            {
                gameplayScreen.NextLevelSequence();
            }
        }
    }
}