using PrimeTween;
using UnityEngine;
using UnityEngine.UI;

namespace View.Puzzle
{
    public class SelectionLine : MonoBehaviour
    {
        [SerializeField] private Image _lineImage;
        [SerializeField] private Image _startCapImage;
        [SerializeField] private Image _endCapImage;
        private static Color transparentColor = new Color(0, 0, 0, 0);
        private Sequence _colorTween;

        private void OnEnable()
        {
            _lineImage.color = transparentColor;
        }

        public void SetColor(Color color, bool immediate = true)
        {
            if (immediate)
            {
                _lineImage.color = color;
                _startCapImage.color = color;
                _endCapImage.color = color;
                return;
            }

            if (_colorTween.isAlive)
            {
                _colorTween.Stop();
            }

            if (_lineImage.color != color)
            {
                _colorTween = Sequence.Create()
                    .Group(Tween.Color(_lineImage, color, 0.2f))
                    .Group(Tween.Color(_startCapImage, color, 0.2f))
                    .Group(Tween.Color(_endCapImage, color, 0.2f));
            }
        }

        public void SetStartCap(Vector3 position)
        {
            _startCapImage.transform.position = position;
            _startCapImage.gameObject.SetActive(true);
            _endCapImage.gameObject.SetActive(false);
            _lineImage.gameObject.SetActive(false);
        }

        public void SetEndCap(Vector3 position)
        {
            _endCapImage.transform.position = position;
            _endCapImage.gameObject.SetActive(true);
            var startPos = _startCapImage.transform.position;
            _lineImage.transform.position = (position + startPos) * 0.5f;
            var rt = (RectTransform)_lineImage.transform;
            rt.right = position - startPos;
            _lineImage.gameObject.SetActive(true);
            SetLineImageWidth();
        }

        private void SetLineImageWidth()
        {
            var lineRt = (RectTransform)_lineImage.transform;
            var parentRt = lineRt.parent as RectTransform;

            var startCapRt = (RectTransform)_startCapImage.transform;
            var endCapRt = (RectTransform)_endCapImage.transform;

            // Convert cap world positions into the line parent's local space.
            Vector2 startLocal = parentRt.InverseTransformPoint(startCapRt.position);
            Vector2 endLocal = parentRt.InverseTransformPoint(endCapRt.position);

            Vector2 dir = endLocal - startLocal;
            float dist = dir.magnitude;
            if (dist <= 0.001f)
            {
                dist = 1f;
                dir = Vector2.right;
            }
            else
            {
                dir /= dist;
            }

            // float startOffset = Mathf.Max(startCapRt.rect.width, startCapRt.rect.height) * 0.25f;
            float startOffset = 0;

            lineRt.pivot = new Vector2(0f, 0.5f);

            lineRt.anchoredPosition = startLocal + dir * startOffset;

            float width = Mathf.Max(1f, dist - startOffset);
            lineRt.sizeDelta = new Vector2(width, lineRt.sizeDelta.y);
        }
    }
}