using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using PrimeTween;
using Service.Puzzle;
using Shared;
using Shared.Service.SharedCoroutine;
using TMPro;
using UnityEngine;
using UnityEngine.Pool;
using Random = UnityEngine.Random;

namespace View.Puzzle
{
    public class TargetWordsView : MonoBehaviour, ISharedUtility
    {
        [SerializeField] private RectTransform _lettersParent;
        private List<TargetWord> _targetWords => _puzzleView.TargetWords;
        [SerializeField] private TextMeshProUGUI _letterPrefab;
        [SerializeField] private TargetWordUI _targetWordUIPrefab;
        [SerializeField] private TextMeshProUGUI _text;
        [SerializeField] private CompletedEffectsView _completedEffectsView;
        private StringBuilder _sb = new StringBuilder();
        private ObjectPool<TextMeshProUGUI> _letterPool;
        private ObjectPool<TargetWordUI> _targetWordUIPool;
        private List<TextMeshProUGUI> _activeLetterAnims = new List<TextMeshProUGUI>();
        private List<TargetWordUI> _activeTargetWordUIs = new List<TargetWordUI>();
        private PuzzleBuilder _puzzleBuilder;
        private PuzzleView _puzzleView;
        private int _lastCompletedWordIndex = -1;
        private Action _onCompleted;

        public static bool LettersAnimPlaying = false;

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
                createFunc: () => Instantiate(_targetWordUIPrefab, transform),
                actionOnGet: ui => ui.gameObject.SetActive(true),
                actionOnRelease: ui => ui.gameObject.SetActive(false),
                actionOnDestroy: ui => Destroy(ui.gameObject),
                collectionCheck: false,
                defaultCapacity: 10,
                maxSize: 100
            );
        }

        public void SpawnWords()
        {
            foreach (var word in _activeTargetWordUIs)
            {
                _targetWordUIPool.Release(word);
            }

            _activeTargetWordUIs.Clear();
            _sb.Clear();

            for (int i = 0; i < _targetWords.Count; i++)
            {
                var targetWordUI = _targetWordUIPool.Get();
                _activeTargetWordUIs.Add(targetWordUI);
                var text = _targetWords[i].Word;
                targetWordUI.SetText(text);

                if (i == 0)
                {
                    _sb.Append(text);
                }
                else
                {
                    _sb.Append("  ");
                    _sb.Append(text);
                }
            }

            _text.SetText(_sb);
            _text.ForceMeshUpdate(true, true);

            this.StartSharedCoroutine(UpdatePositions());

            IEnumerator UpdatePositions()
            {
                yield return _waitForEndOfFrame;
                // Position target word UIs to match the _text
                var charInfo = _text.textInfo.characterInfo;
                for (int i = 0; i < _activeTargetWordUIs.Count; i++)
                {
                    var targetWordUI = _activeTargetWordUIs[i];
                    var targetWord = _targetWords[i].Word;
                    var startCharIndex = _sb.ToString().IndexOf(targetWord, StringComparison.Ordinal);
                    if (startCharIndex < 0) continue;
                    targetWordUI.transform.position = _text.transform.TransformPoint(
                        (charInfo[startCharIndex].bottomLeft +
                         charInfo[startCharIndex + targetWord.Length - 1].topRight) *
                        0.5f
                    );
                    targetWordUI.FontSize = _text.fontSize;
                    targetWordUI.SetCompleted(false);
                }
            }
        }

        private WaitForEndOfFrame _waitForEndOfFrame = new WaitForEndOfFrame();

        public void SyncWordsCompleted()
        {
            var foundWords = _puzzleView.FoundTargetWordIndices;
            for (int i = 0; i < _activeTargetWordUIs.Count; i++)
            {
                var targetWordUI = _activeTargetWordUIs[i];
                targetWordUI.SetCompleted(foundWords.Contains(i));
            }
        }


        private void InvokeOnCompleted()
        {
            _onCompleted?.Invoke();
            _onCompleted = null;
            LettersAnimPlaying = false;
        }

        private void ReleaseActiveLetterAnims()
        {
            foreach (var letter in _activeLetterAnims)
            {
                _letterPool.Release(letter);
            }

            _activeLetterAnims.Clear();
        }

        private void TryPlayCompletedEffect()
        {
            _completedEffectsView.TryPlayCompletedEffects(_activeTargetWordUIs[_lastCompletedWordIndex].transform.position);
        }

        private Tween _callbackTween;

        public void PlayAnimWordCompleted(List<int> cellIndices, string word, bool isReversed, Action onComplete = null)
        {
            _onCompleted = onComplete;
            LettersAnimPlaying = true;
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
            _lastCompletedWordIndex = wordListIndex;

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


            if (_callbackTween.isAlive)
            {
                _callbackTween.Stop();
            }

            _callbackTween = Tween.Delay(0.61f).OnComplete(this, x =>
            {
                x.ReleaseActiveLetterAnims();
                x.SyncWordsCompleted();
                x.InvokeOnCompleted();
                x.TryPlayCompletedEffect();
            });
        }
    }
}