using System;
using System.Collections;
using System.Collections.Generic;
using Entity.PuzzleSaveData;
using Service.Gameplay;
using Service.Puzzle;
using Shared;
using Shared.Core.IoC;
using Shared.Service.SharedCoroutine;
using TMPro;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.UI;
using Zenject;

namespace View.Puzzle
{
    [DisallowMultipleComponent]
    public class PuzzleView : IoCMonoBehavior, ISharedUtility
    {
        private const string Tag = "PuzzleView";

        public enum BoardOrientation
        {
            Normal,
            Rot180
        }

        public BoardOrientation boardOrientation = BoardOrientation.Normal;

        [Inject] private PuzzleService _puzzleService;
        [Inject] private GameplayService _gameplayService;

        [Header("Scene References")] public GridLayoutGroup gridLayoutGroup;
        public PuzzleCell cellPrefab;
        public SelectionLine SelectionLineHandlerPrefab;
        public TMP_Text titleLabel;
        public TargetWordsView _targetWordsView;
        public RectTransform _linesContainer;
        public Button _flipBoardButton;

        [SerializeField] private PuzzleInputHandler _puzzleInputHandler;
        public SelectedWordView _selectedWordView;
        private PuzzleBuilder _puzzleBuilder;
        private SelectionLineHandler _selectionLineHandler;
        private CellSelector _cellSelector;

        private List<TargetWord> _targetWords = new List<TargetWord>();
        public List<TargetWord> TargetWords => _targetWords;
        private HashSet<int> _foundTargetWordIndices = new HashSet<int>();
        public HashSet<int> FoundTargetWordIndices => _foundTargetWordIndices;
        public bool IsRotating { get; private set; }
        public bool WordsCompleted { get; private set; }

        protected override void Awake()
        {
            base.Awake();
            var cellPool = new ObjectPool<PuzzleCell>(
                createFunc: () => Instantiate(cellPrefab, gridLayoutGroup.transform),
                actionOnGet: cell => cell.gameObject.SetActive(true),
                actionOnRelease: cell => cell.gameObject.SetActive(false),
                actionOnDestroy: cell => Destroy(cell.gameObject),
                collectionCheck: false,
                defaultCapacity: 25,
                maxSize: 100
            );
            var linePool = new ObjectPool<SelectionLine>(
                createFunc: () => Instantiate(SelectionLineHandlerPrefab, _linesContainer),
                actionOnGet: line => line.gameObject.SetActive(true),
                actionOnRelease: line => line.gameObject.SetActive(false),
                actionOnDestroy: line => Destroy(line.gameObject),
                collectionCheck: false,
                defaultCapacity: 5,
                maxSize: 20
            );
            _puzzleBuilder = new PuzzleBuilder(cellPool, gridLayoutGroup);
            _selectionLineHandler = new SelectionLineHandler(linePool);
            _cellSelector = new CellSelector(_puzzleBuilder, _puzzleService, this, _selectionLineHandler,
                _selectedWordView, _targetWordsView);
            _targetWordsView.Init(_puzzleBuilder, this);
            _selectedWordView.Init(_selectionLineHandler);
        }

        private void Start()
        {
            _cellSelector.RegisterInputHandler(_puzzleInputHandler);
            _flipBoardButton.onClick.AddListener(OnFlipBoardButtonPressed);
        }

        private void OnFlipBoardButtonPressed()
        {
            if (IsRotating || WordsCompleted) return;
            // Prevent mid-drag mismatches; the current selection is not re-projected.
            _selectionLineHandler?.ClearSelectedLine();
            _selectedWordView?.SetWord(null);

            boardOrientation = boardOrientation == BoardOrientation.Normal
                ? BoardOrientation.Rot180
                : BoardOrientation.Normal;

            // ApplyBoardOrientationVisuals();
            this.StartSharedCoroutine(RotateBoardRoutine());
            var save = _puzzleService.SaveData;
            save.IsBoardRotated = boardOrientation == BoardOrientation.Rot180;
            _puzzleService.SavePuzzleData(save);
        }

        private IEnumerator RotateBoardRoutine()
        {
            if (gridLayoutGroup == null)
            {
                IsRotating = false;
                yield break;
            }

            IsRotating = true;

            var boardRect = (RectTransform)gridLayoutGroup.transform;
            var linesRect = _linesContainer;

            var targetZ = boardOrientation == BoardOrientation.Rot180 ? 180f : 360f;

            // Cache starts
            var boardStartScale = boardRect.localScale;
            var linesStartScale = linesRect.localScale;

            var boardStartRot = boardRect.localRotation;
            var linesStartRot = linesRect.localRotation;

            var boardTargetRot = Quaternion.Euler(0f, 0f, targetZ);
            var linesTargetRot = Quaternion.Euler(0f, 0f, targetZ);

            const float scalePortion = 0.30f;
            const float rotatePortion = 0.70f;
            const float totalDuration = 0.8f;
            const float shrinkScaleMul = 0.8f;

            var shrinkDuration = Mathf.Max(0.01f, totalDuration * (scalePortion * 0.5f));
            var rotateDuration = Mathf.Max(0.01f, totalDuration * rotatePortion);
            var expandDuration = shrinkDuration;

            var shrinkScale = boardStartScale * shrinkScaleMul;
            var shrinkScaleLines = linesStartScale * shrinkScaleMul;

            // Phase 1: shrink
            for (float t = 0f; t < shrinkDuration; t += Time.unscaledDeltaTime)
            {
                var a = Mathf.SmoothStep(0f, 1f, t / shrinkDuration);

                boardRect.localScale = Vector3.Lerp(boardStartScale, shrinkScale, a);

                linesRect.localScale = Vector3.Lerp(linesStartScale, shrinkScaleLines, a);

                yield return null;
            }

            boardRect.localScale = shrinkScale;
            linesRect.localScale = shrinkScaleLines;

            // Phase 2: rotate to target
            for (float t = 0f; t < rotateDuration; t += Time.unscaledDeltaTime)
            {
                var a = Mathf.SmoothStep(0f, 1f, t / rotateDuration);

                boardRect.localRotation = Quaternion.Slerp(boardStartRot, boardTargetRot, a);

                linesRect.localRotation = Quaternion.Slerp(linesStartRot, linesTargetRot, a);

                // Keep cells visually upright relative to the board (match board rotation).
                for (var i = _puzzleBuilder.Cells.Count - 1; i >= 0; i--)
                {
                    var cell = _puzzleBuilder.Cells[i];
                    cell.transform.localRotation = Quaternion.Euler(0f, 0f,
                        Mathf.LerpAngle(boardStartRot.eulerAngles.z, targetZ, a));
                }

                yield return null;
            }

            boardRect.localRotation = boardTargetRot;
            linesRect.localRotation = linesTargetRot;

            // Phase 3: expand back
            for (float t = 0f; t < expandDuration; t += Time.unscaledDeltaTime)
            {
                var a = Mathf.SmoothStep(0f, 1f, t / expandDuration);
                boardRect.localScale = Vector3.Lerp(shrinkScale, boardStartScale, a);

                linesRect.localScale = Vector3.Lerp(shrinkScaleLines, linesStartScale, a);

                yield return null;
            }

            boardRect.localScale = boardStartScale;
            linesRect.localScale = linesStartScale;

            // Snap final visuals to avoid drift.
            ApplyBoardOrientationVisuals();

            IsRotating = false;
        }

        private void ApplyBoardOrientationVisuals()
        {
            var zRot = boardOrientation == BoardOrientation.Rot180 ? 180f : 0f;

            var t = gridLayoutGroup.transform;
            t.localRotation = Quaternion.Euler(0f, 0f, zRot);
            
            _linesContainer.localRotation = Quaternion.Euler(0f, 0f, zRot);

            for (var i = _puzzleBuilder.Cells.Count - 1; i >= 0; i--)
            {
                var cell = _puzzleBuilder.Cells[i];
                cell.transform.localRotation = Quaternion.Euler(0f, 0f, zRot);
            }
        }

        public void SetupPuzzle(GamePuzzle level)
        {
            // Reset per-level state so subsequent levels don't inherit stale data.
            WordsCompleted = false;
            IsRotating = false;

            // Clear any in-progress selection / UI from previous level.
            _selectionLineHandler.ClearSelectedLine();
            _selectedWordView.SetWord(null);

            // Reset found/target words collections before we add the new level data.
            _targetWords.Clear();
            _foundTargetWordIndices.Clear();

            // Reset orientation to a canonical state for new level visuals.
            boardOrientation = BoardOrientation.Normal;
            ApplyBoardOrientationVisuals();

            // Now apply the new level content.
            titleLabel.text = level.Title;
            _puzzleBuilder.SetupPuzzle(level);
            _cellSelector.SetLevel(level);
            _targetWords.AddRange(level.TargetWords);

            _selectionLineHandler.OnNewPuzzle();
            _targetWordsView.SpawnWords();
        }

        public void LoadData(PuzzleSaveDataEntity puzzleServiceSaveData)
        {
            var solvedWords = puzzleServiceSaveData.SolvedWords;
            _foundTargetWordIndices.Clear();
            for (int i = 0; i < solvedWords.Count; i++)
            {
                var word = solvedWords[i];
                var idx = word.WordIndex;
                if (idx < 0 || idx >= _targetWords.Count) continue;

                _foundTargetWordIndices.Add(idx);
                _puzzleBuilder.GetWordStartEnd(_targetWords[idx].Word, out var start, out var end);
                _selectionLineHandler.SetCompletedLine(start, end, word.ColorIndex);
            }

            _targetWordsView.SyncWordsCompleted();
        }

        public void AddFoundWord(TargetWord foundWord)
        {
            var idx = _targetWords.IndexOf(foundWord);
            if (idx >= 0)
            {
                _foundTargetWordIndices.Add(idx);
            }

            var save = _puzzleService.SaveData;
            save.SolvedWords.Add(new PuzzleSaveDataEntity.SolvedWord()
            {
                WordIndex = idx,
                ColorIndex = _selectionLineHandler.WordColorsIndex[^1]
            });
            _puzzleService.SavePuzzleData(save);

            this.StartSharedCoroutine(CheckAllWordsFoundRoutine());
        }

        private IEnumerator CheckAllWordsFoundRoutine()
        {
            WordsCompleted = _foundTargetWordIndices.Count == _targetWords.Count;
            yield return null;
            while (TargetWordsView.LettersAnimPlaying || CompletedEffectsView.IsPlayingCompletedEffects)
            {
                yield return null;
            }

            if (WordsCompleted)
            {
                _gameplayService.LevelCompleted();
            }
        }


        public bool ValidateWord(string word, string reversed, out TargetWord foundWord, out bool isReversed)
        {
            foundWord = null;
            isReversed = false;
            var foundIdx = -1;
            for (var i = 0; i < _targetWords.Count; i++)
            {
                var targetWord = _targetWords[i];
                if (string.Equals(word, targetWord.Word, StringComparison.OrdinalIgnoreCase))
                {
                    foundWord = targetWord;
                    isReversed = false;
                    foundIdx = i;
                    break;
                }

                if (string.Equals(reversed, targetWord.Word, StringComparison.OrdinalIgnoreCase))
                {
                    foundWord = targetWord;
                    isReversed = true;
                    foundIdx = i;
                    break;
                }
            }

            if (foundIdx >= 0 && _foundTargetWordIndices.Contains(foundIdx))
            {
                foundWord = null;
                return false;
            }

            return foundWord != null;
        }
    }
}

