using DG.Tweening;
using PrimeTween;
using UnityEngine;
using UnityEngine.UI;
using Sequence = PrimeTween.Sequence;
using Tween = PrimeTween.Tween;

namespace View.Puzzle
{
    public class SelectionLine : MonoBehaviour
    {
        [SerializeField] private Image _lineImage;
        [SerializeField] private Image _startCapImage;
        [SerializeField] private Image _endCapImage;
        private static Color transparentColor = new Color(0, 0, 0, 0);
        private Sequence _colorTween;
        public Color Color { get; private set; }

        public float Size;


#if UNITY_EDITOR
        private void Update()
        {
            _lineImage.rectTransform.sizeDelta = new Vector2(_lineImage.rectTransform.sizeDelta.x, Size);
            _startCapImage.rectTransform.sizeDelta = new Vector2(Size, Size);
            _endCapImage.rectTransform.sizeDelta = new Vector2(Size, Size);
        }

#endif
        public void PlayLineComplete()
        {
            Vector3 punch = new Vector3(1.1f, 1.1f, 1.1f);
            float duration = 0.1f;
            PrimeTween.Sequence.Create()
                .Group(Tween.Scale(transform, punch, duration, PrimeTween.Ease.InBack))
                .Chain(Tween.Scale(transform, Vector3.one, duration, PrimeTween.Ease.InQuad));
        }

        private void Start()
        {
            _lineImage.rectTransform.sizeDelta = new Vector2(_lineImage.rectTransform.sizeDelta.x, Size);
            _startCapImage.rectTransform.sizeDelta = new Vector2(Size, Size);
            _endCapImage.rectTransform.sizeDelta = new Vector2(Size, Size);
        }

        public void SetColor(Color color, bool immediate = true)
        {
            Color = color;
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

        private Vector3[] _worldCorners = new Vector3[4];

        public void SetSurround(RectTransform rectTransform)
        {
            rectTransform.GetWorldCorners(_worldCorners);
            SetStartCap((_worldCorners[0] + _worldCorners[1]) * 0.5f);
            SetEndCap((_worldCorners[2] + _worldCorners[3]) * 0.5f);
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
            SyncRootTransform();
        }

        private void SyncRootTransform()
        {
            var lineT = _lineImage.transform;
            var startT = _startCapImage.transform;
            var endT = _endCapImage.transform;
            var linePos = lineT.position;
            var lineRot = lineT.rotation;
            var startPos = startT.position;
            var endPos = endT.position;

            transform.SetPositionAndRotation((startPos + endPos) * 0.5f, lineRot);
            startT.position = startPos;
            endT.position = endPos;
            _lineImage.transform.SetPositionAndRotation(linePos, lineRot);
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