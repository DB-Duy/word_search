using PrimeTween;
using TMPro;
using UnityEngine;

namespace View.Puzzle
{
    public class SelectedWordView : MonoBehaviour
    {
        [SerializeField] [HideInInspector] private RectTransform _rectTransform;
        [SerializeField] private TextMeshProUGUI _wordText;
        [SerializeField] private SelectionLine _line;
        [SerializeField] public Vector2 padding;
        public float paddingX => padding.x;
        public float paddingY => padding.y;
        private string _currentWord;
        private SelectionLineHandler _selectionLineHandler;
        private Sequence _sequence;

        public void Init(SelectionLineHandler selectionLineHandler)
        {
            _selectionLineHandler = selectionLineHandler;
            Hide();
        }

        private void OnValidate()
        {
            _rectTransform = _wordText.GetComponent<RectTransform>();
        }

        public void SetWord(string word)
        {
            if (string.IsNullOrEmpty(word))
            {
                Hide();
                return;
            }

            if (_currentWord != word)
            {
                var c = _selectionLineHandler.GetCurrentLineColor();
                _line.SetColor(c, true);
                _currentWord = word;
                _wordText.SetText(word);
                if (!gameObject.activeSelf) gameObject.SetActive(true);
                Resize();
            }
        }

        public void PlayIncorrectAnimation()
        {
            if (!gameObject.activeInHierarchy) return;
            if(_sequence.isAlive) _sequence.Stop();
            _sequence = Sequence.Create()
                .Group(Tween.ShakeLocalPosition(transform, new Vector3(10f, 0, 0), 0.5f, enableFalloff: false))
                .OnComplete(this, (x) => x.Hide());
        }

        private void Hide()
        {
            gameObject.SetActive(false);
            var pos = transform.localPosition;
            pos.x = 0;
            transform.localPosition = pos;
        }

        private void Resize()
        {
            Vector2 targetSize = new Vector2(
                _wordText.preferredWidth + paddingX,
                _wordText.preferredHeight + paddingY
            );
            if (!Mathf.Approximately(_rectTransform.sizeDelta.x, targetSize.x))
            {
                // Tween.UISizeDelta(_rectTransform, targetSize, 0.1f);
                _rectTransform.sizeDelta = targetSize;
                _line.SetSurround(_rectTransform);
            }
        }
    }
}