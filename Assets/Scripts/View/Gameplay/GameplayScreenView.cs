using System;
using System.Collections;
using PrimeTween;
using Service.Puzzle;
using Service.UserData;
using Shared;
using Shared.Core.IoC;
using Shared.Core.View.Screen;
using Shared.Service.SharedCoroutine;
using UnityEngine;
using View.Puzzle;
using Zenject;

namespace View.Gameplay
{
    public class GameplayScreenView : IoCMonoBehavior, IUIScreen, ISharedUtility
    {
        [SerializeField] private PuzzleView _puzzleView;
        [SerializeField] private CanvasGroup _puzzleViewCanvasGroup;
        [Inject] private PuzzleService _puzzleService;
        [Inject] private UserDataService _userDataService;

        protected override void Awake()
        {
            base.Awake();
            Input.multiTouchEnabled = false;
        }

        private void Start()
        {
            _puzzleViewCanvasGroup.alpha = 0;
            StartLevel();
        }

        private IEnumerator ShowPuzzleBoard()
        {
            _puzzleView.SetupPuzzle(_puzzleService.GetPuzzleById(_userDataService.DisplayLevel - 1));
            if (_puzzleService.HasSaveData)
            {
                // wait 2 frames for puzzle create
                yield return null;
                yield return null;
                _puzzleView.LoadData(_puzzleService.SaveData);
            }

            Tween.Alpha(_puzzleViewCanvasGroup, 1f, 0.2f);
        }
        [ContextMenu("Show Puzzle Board")]
        public void NextLevelSequence()
        {
            var puzzleViewT = _puzzleView.transform;
            Sequence seq = Sequence.Create()
                .Chain(Tween.Scale(puzzleViewT, 1.2f * Vector3.one, 0.1f, Ease.OutBounce))
                .Chain(Tween.Scale(puzzleViewT, Vector3.zero, 1f, Ease.Linear))
                .Group(Tween.Rotation(puzzleViewT, puzzleViewT.eulerAngles + new Vector3(0, 0, -180), 1/3f, Ease.Linear, 3,
                    CycleMode.Incremental))
                .Group(Tween.Alpha(_puzzleViewCanvasGroup, 0, 0.5f, Ease.Linear, startDelay: 0.5f))
                .OnComplete(this, (x) => x.StartLevel());
        }

        private void StartLevel()
        {
            _puzzleView.transform.localScale = Vector3.one;
            _puzzleView.transform.rotation = Quaternion.identity;
            this.StartSharedCoroutine(ShowPuzzleBoard());
        }
    }
}