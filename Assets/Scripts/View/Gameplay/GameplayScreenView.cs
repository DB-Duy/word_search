using System;
using Service.Puzzle;
using Shared.Core.IoC;
using Shared.Core.View.Screen;
using UnityEngine;
using View.Puzzle;
using Zenject;

namespace View.Gameplay
{
    public class GameplayScreenView : IoCMonoBehavior, IUIScreen
    {
        [SerializeField] private PuzzleView _puzzleView;
        [Inject] private PuzzleService _puzzleService;

        private void Start()
        {
            _puzzleView.SetupPuzzle(_puzzleService.GetPuzzleById(4));
        }
        
    }
}