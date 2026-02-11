using System;
using System.Collections.Generic;
using Service.Puzzle;
using Shared.Core.IoC;
using TMPro;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.UI;
using Zenject;

namespace View.Puzzle
{
    [DisallowMultipleComponent]
    public class PuzzleView : IoCMonoBehavior
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

        public PuzzleInputHandler _inputHandler;
        public SelectedWordView _selectedWordView;
        private PuzzleBuilder _puzzleBuilder;
        private SelectionLineHandler _selectionLineHandler;
        private CellSelector _cellSelector;

        private List<TargetWord> _targetWords = new List<TargetWord>();
        private HashSet<int> _foundTargetWordIndices = new HashSet<int>();
        public HashSet<int> FoundTargetWordIndices => _foundTargetWordIndices;

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

            // Ensure current inspector value is applied visually.
            ApplyBoardOrientationVisuals();
        }

        private void OnFlipBoardButtonPressed()
        {
            // Prevent mid-drag mismatches; the current selection is not re-projected.
            _selectionLineHandler?.ClearSelectedLine();
            _selectedWordView?.SetWord(null);

            boardOrientation = boardOrientation == BoardOrientation.Normal
                ? BoardOrientation.Rot180
                : BoardOrientation.Normal;

            ApplyBoardOrientationVisuals();
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
            _targetWordsView.SetTargetWords(_targetWords);
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