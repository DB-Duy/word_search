using System;
using System.Collections.Generic;
using System.Text;
using Service.Puzzle;
using TMPro;
using UnityEngine;
using UnityEngine.Pool;

namespace View.Puzzle
{
    public class TargetWordsView : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _targetWordsText;
        private static StringBuilder _sb = new StringBuilder();
        private List<string> _targetWords = new List<string>();
        private const string CompletedWordFormat = "<color=#ABABAB>{0}</color>";
        [SerializeField] private TextMeshProUGUI _letterPrefab;
        private ObjectPool<TextMeshProUGUI> _letterPool;
        private List<TextMeshProUGUI> _activeLetterAnims = new List<TextMeshProUGUI>();
        private PuzzleBuilder _puzzleBuilder;
        private PuzzleView _puzzleView;

        private Action _onCompleted;

        public void Init(PuzzleBuilder puzzleBuilder, PuzzleView puzzleView)
        {
            _puzzleBuilder = puzzleBuilder;
            _puzzleView = puzzleView;
            _letterPool = new ObjectPool<TextMeshProUGUI>(
                createFunc: () => Instantiate(_letterPrefab, _targetWordsText.transform),
                actionOnGet: letter => letter.gameObject.SetActive(true),
                actionOnRelease: letter => letter.gameObject.SetActive(false),
                actionOnDestroy: letter => Destroy(letter.gameObject),
                collectionCheck: false,
                defaultCapacity: 10,
                maxSize: 100
            );
        }

        public void SetTargetWords(List<TargetWord> targetWords)
        {
            if (targetWords == null || targetWords.Count == 0)
            {
                return;
            }

            _sb.Clear();
            _sb.Append(targetWords[0].Word);
            _targetWords.Clear();
            _targetWords.Add(targetWords[0].Word);

            for (int i = 1; i < targetWords.Count; i++)
            {
                var targetWord = targetWords[i];
                _targetWords.Add(targetWord.Word);
                _sb.Append(" ").Append(targetWord.Word);
            }

            _targetWordsText.SetText(_sb);
            _targetWordsText.ForceMeshUpdate();
        }

        private void SyncWordsVisual()
        {
            var foundWords = _puzzleView.FoundTargetWordIndices;
            _sb.Clear();

            for (int i = 0; i < _targetWords.Count; i++)
            {
                if (foundWords.Contains(i))
                {
                    _sb.AppendFormat(CompletedWordFormat, _targetWords[i]);
                }
                else
                {
                    _sb.Append(_targetWords[i]);
                }

                if (i < _targetWords.Count - 1)
                {
                    _sb.Append(" ");
                }
            }

            _targetWordsText.SetText(_sb);
            _targetWordsText.ForceMeshUpdate();
        }

        private void InvokeOnCompleted()
        {
            _onCompleted?.Invoke();
            _onCompleted = null;
        }

        private int GetWordStartCharIndex(int wordListIndex)
        {
            // Words are displayed as: word0 + (space + word1) + ...
            var start = 0;
            for (int i = 0; i < wordListIndex; i++)
            {
                start += _targetWords[i].Length + 1; // +1 for the space delimiter
            }

            return start;
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

            _targetWordsText.ForceMeshUpdate();

            var wordListIndex = -1;
            for (int i = 0; i < _targetWords.Count; i++)
            {
                if (string.Equals(_targetWords[i], word, StringComparison.OrdinalIgnoreCase))
                {
                    wordListIndex = i;
                    break;
                }
            }

            var wordStartIndex = GetWordStartCharIndex(wordListIndex);
            var charInfo = _targetWordsText.textInfo.characterInfo;
            var cells = _puzzleBuilder.Cells;

            for (int i = 0; i < cellIndices.Count; i++)
            {
                var index = isReversed ? cellIndices.Count - 1 - i : i;
                var letter = _letterPool.Get();
                var cell = cells[cellIndices[index]];

                letter.text = cell.Letter.ToString();
                letter.transform.position = cell.transform.position;
                letter.fontSize = cell.FontSize;

                var targetCharInfo = charInfo[wordStartIndex + i];
                var targetPos = _targetWordsText.transform.TransformPoint(
                    (targetCharInfo.bottomLeft + targetCharInfo.topRight) * 0.5f
                );
                _activeLetterAnims.Add(letter);
                PrimeTween.Tween.Position(letter.transform, targetPos, 0.6f);
            }

            PrimeTween.Tween.Delay(0.61f).OnComplete(this, x =>
            {
                x.ReleaseActiveLetterAnims();
                x.SyncWordsVisual();
                x.InvokeOnCompleted();
            });
        }
    }
}