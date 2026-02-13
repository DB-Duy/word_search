using System;
using System.Collections.Generic;
using PrimeTween;
using Service.Puzzle;
using Shared;
using TMPro;
using UnityEngine;
using UnityEngine.Pool;

namespace View.Puzzle
{
    public class TargetWordsView : MonoBehaviour, ISharedUtility
    {
        [SerializeField] private RectTransform[] _wordsParentLines;
        [SerializeField] private RectTransform _lettersParent;
        private List<TargetWord> _targetWords => _puzzleView.TargetWords;
        private const string CompletedWordFormat = "<color=#727272>{0}</color>";
        [SerializeField] private TextMeshProUGUI _letterPrefab;
        [SerializeField] private TargetWordUI _targetWordUIPrefab;
        private ObjectPool<TextMeshProUGUI> _letterPool;
        private ObjectPool<TargetWordUI> _targetWordUIPool;
        private List<TextMeshProUGUI> _activeLetterAnims = new List<TextMeshProUGUI>();
        private List<TargetWordUI> _activeTargetWordUIs = new List<TargetWordUI>();
        private PuzzleBuilder _puzzleBuilder;
        private PuzzleView _puzzleView;

        private Action _onCompleted;

        public void Init(PuzzleBuilder puzzleBuilder, PuzzleView puzzleView)
        {
            _puzzleBuilder = puzzleBuilder;
            _puzzleView = puzzleView;
            _letterPool = new ObjectPool<TextMeshProUGUI>(
                createFunc: () => Instantiate(_letterPrefab, _lettersParent),
                actionOnGet: letter => letter.gameObject.SetActive(true),
                actionOnRelease: letter => letter.gameObject.SetActive(false),
                actionOnDestroy: letter => Destroy(letter.gameObject),
                collectionCheck: false,
                defaultCapacity: 10,
                maxSize: 100
            );
            _targetWordUIPool = new ObjectPool<TargetWordUI>(
                createFunc: () => Instantiate(_targetWordUIPrefab, _wordsParentLines[0]),
                actionOnGet: ui => ui.gameObject.SetActive(true),
                actionOnRelease: ui => ui.gameObject.SetActive(false),
                actionOnDestroy: ui => Destroy(ui.gameObject),
                collectionCheck: false,
                defaultCapacity: 10,
                maxSize: 100
            );
        }

        public void SyncWordsVisual()
        {
            var foundWords = _puzzleView.FoundTargetWordIndices;

            foreach (var word in _activeTargetWordUIs)
            {
                _targetWordUIPool.Release(word);
            }

            _activeTargetWordUIs.Clear();

            for (int i = 0; i < _targetWords.Count; i++)
            {
                var targetWordUI = _targetWordUIPool.Get();
                _activeTargetWordUIs.Add(targetWordUI);
                var text = foundWords.Contains(i)
                    ? string.Format(CompletedWordFormat, _targetWords[i].Word)
                    : _targetWords[i].Word;
                targetWordUI.SetText(text);
            }
        }


        private void InvokeOnCompleted()
        {
            _onCompleted?.Invoke();
            _onCompleted = null;
        }

        private void ReleaseActiveLetterAnims()
        {
            foreach (var letter in _activeLetterAnims)
            {
                _letterPool.Release(letter);
            }

            _activeLetterAnims.Clear();
        }

        public void PlayAnimWordCompleted(List<int> cellIndices, string word, bool isReversed, Action onComplete = null)
        {
            _onCompleted = onComplete;
            var wordListIndex = -1;
            for (int i = 0; i < _targetWords.Count; i++)
            {
                if (string.Equals(_targetWords[i].Word, word, StringComparison.OrdinalIgnoreCase))
                {
                    wordListIndex = i;
                    break;
                }
            }

            var targetWordUI = _activeTargetWordUIs[wordListIndex];

            var charInfo = targetWordUI.TextInfo.characterInfo;
            var cells = _puzzleBuilder.Cells;

            for (int i = 0; i < cellIndices.Count; i++)
            {
                var index = isReversed ? cellIndices.Count - 1 - i : i;
                var letter = _letterPool.Get();
                var cell = cells[cellIndices[index]];

                letter.text = cell.Letter.ToString();
                letter.transform.position = cell.transform.position;
                letter.fontSize = cell.FontSize;

                var targetCharInfo = charInfo[i];
                var targetPos = targetWordUI.transform.TransformPoint(
                    (targetCharInfo.bottomLeft + targetCharInfo.topRight) * 0.5f
                );
                _activeLetterAnims.Add(letter);
                Tween.Position(letter.transform, targetPos, 0.6f, Ease.OutSine);
            }

            Tween.Delay(0.61f).OnComplete(this, x =>
            {
                x.ReleaseActiveLetterAnims();
                x.SyncWordsVisual();
                x.InvokeOnCompleted();
            });
        }
    }
}