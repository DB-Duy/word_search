using System;
using System.Collections;
using System.Collections.Generic;
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
        }

        private void Start()
        {
            _cellSelector.RegisterInputHandler(_puzzleInputHandler);
            _flipBoardButton.onClick.AddListener(OnFlipBoardButtonPressed);
        }

        private void OnFlipBoardButtonPressed()
        {
            if (IsRotating) return;
            // Prevent mid-drag mismatches; the current selection is not re-projected.
            _selectionLineHandler?.ClearSelectedLine();
            _selectedWordView?.SetWord(null);

            boardOrientation = boardOrientation == BoardOrientation.Normal
                ? BoardOrientation.Rot180
                : BoardOrientation.Normal;

            // ApplyBoardOrientationVisuals();
            this.StartSharedCoroutine(RotateBoardRoutine());
        }

        private IEnumerator RotateBoardRoutine()
        {
            if (gridLayoutGroup == null)
            {
                IsRotating = false;
                yield break;
            }

            IsRotating = true;

            var boardRect = gridLayoutGroup.transform as RectTransform;
            var linesRect = _linesContainer;

            var targetZ = boardOrientation == BoardOrientation.Rot180 ? 180f : 360f;

            // Cache starts
            var boardStartScale = boardRect != null ? boardRect.localScale : gridLayoutGroup.transform.localScale;
            var linesStartScale = linesRect != null ? linesRect.localScale : Vector3.one;

            var boardStartRot = boardRect != null ? boardRect.localRotation : gridLayoutGroup.transform.localRotation;
            var linesStartRot = linesRect != null ? linesRect.localRotation : Quaternion.identity;

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

                if (boardRect != null) boardRect.localScale = Vector3.Lerp(boardStartScale, shrinkScale, a);
                else gridLayoutGroup.transform.localScale = Vector3.Lerp(boardStartScale, shrinkScale, a);

                if (linesRect != null) linesRect.localScale = Vector3.Lerp(linesStartScale, shrinkScaleLines, a);

                yield return null;
            }

            if (boardRect != null) boardRect.localScale = shrinkScale;
            else gridLayoutGroup.transform.localScale = shrinkScale;
            if (linesRect != null) linesRect.localScale = shrinkScaleLines;

            // Phase 2: rotate to target
            for (float t = 0f; t < rotateDuration; t += Time.unscaledDeltaTime)
            {
                var a = Mathf.SmoothStep(0f, 1f, t / rotateDuration);

                if (boardRect != null) boardRect.localRotation = Quaternion.Slerp(boardStartRot, boardTargetRot, a);
                else gridLayoutGroup.transform.localRotation = Quaternion.Slerp(boardStartRot, boardTargetRot, a);

                if (linesRect != null) linesRect.localRotation = Quaternion.Slerp(linesStartRot, linesTargetRot, a);

                // Keep cells visually upright relative to the board (match board rotation).
                for (var i = _puzzleBuilder.Cells.Count - 1; i >= 0; i--)
                {
                    var cell = _puzzleBuilder.Cells[i];
                    if (cell != null)
                        cell.transform.localRotation = Quaternion.Euler(0f, 0f,
                            Mathf.LerpAngle(boardStartRot.eulerAngles.z, targetZ, a));
                }

                yield return null;
            }

            if (boardRect != null) boardRect.localRotation = boardTargetRot;
            else gridLayoutGroup.transform.localRotation = boardTargetRot;
            if (linesRect != null) linesRect.localRotation = linesTargetRot;

            // Phase 3: expand back
            for (float t = 0f; t < expandDuration; t += Time.unscaledDeltaTime)
            {
                var a = Mathf.SmoothStep(0f, 1f, t / expandDuration);

                if (boardRect != null) boardRect.localScale = Vector3.Lerp(shrinkScale, boardStartScale, a);
                else gridLayoutGroup.transform.localScale = Vector3.Lerp(shrinkScale, boardStartScale, a);

                if (linesRect != null) linesRect.localScale = Vector3.Lerp(shrinkScaleLines, linesStartScale, a);

                yield return null;
            }

            if (boardRect != null) boardRect.localScale = boardStartScale;
            else gridLayoutGroup.transform.localScale = boardStartScale;
            if (linesRect != null) linesRect.localScale = linesStartScale;

            // Snap final visuals to avoid drift.
            ApplyBoardOrientationVisuals();

            IsRotating = false;
        }

        private void ApplyBoardOrientationVisuals()
        {
            var zRot = boardOrientation == BoardOrientation.Rot180 ? 180f : 0f;

            if (gridLayoutGroup != null)
            {
                var t = gridLayoutGroup.transform as RectTransform;
                if (t != null) t.localRotation = Quaternion.Euler(0f, 0f, zRot);
                else gridLayoutGroup.transform.localRotation = Quaternion.Euler(0f, 0f, zRot);
            }

            if (_linesContainer != null)
            {
                _linesContainer.localRotation = Quaternion.Euler(0f, 0f, zRot);
            }

            for (var i = _puzzleBuilder.Cells.Count - 1; i >= 0; i--)
            {
                var cell = _puzzleBuilder.Cells[i];
                if (cell != null)
                {
                    cell.transform.localRotation = Quaternion.Euler(0f, 0f, zRot);
                }
            }
        }

        public void SetupPuzzle(GamePuzzle level)
        {
            titleLabel.text = level.Title;
            _puzzleBuilder.SetupPuzzle(level);
            _cellSelector.SetLevel(level);
            _targetWords.Clear();
            _foundTargetWordIndices.Clear();
            _targetWords.AddRange(level.TargetWords);
            _selectionLineHandler.RefreshColors();
            _selectedWordView.Init(_selectionLineHandler);
            _targetWordsView.SyncWordsVisual();
        }

        public void AddFoundWord(TargetWord foundWord)
        {
            var idx = _targetWords.IndexOf(foundWord);
            if (idx >= 0)
            {
                _foundTargetWordIndices.Add(idx);
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