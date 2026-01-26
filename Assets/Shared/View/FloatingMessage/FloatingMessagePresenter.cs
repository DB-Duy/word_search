using System.Collections;
using DG.Tweening;
using Shared.Utils;
using UnityEngine;

namespace Shared.View.FloatingMessage
{
    public class FloatingMessagePresenter : IFloatingMessagePresenter
    {
        private float? _height;
        private float Height => _height ??= (Screen.height / 4f);
        private readonly float _duration;
        private float _startTime = -1f;
        private RectTransform _lastMessage;

        public FloatingMessagePresenter(float? height = null, float duration = 3f)
        {
            _height = height;
            _duration = duration;
        }

        public IEnumerator Present(MonoBehaviour o)
        {
            if (Time.unscaledTime - _startTime <= 0.5f * _duration) yield break;

            var go = o.gameObject;
            var messageTransform = o.transform;
            if (_IsOverlap(messageTransform as RectTransform)) yield break;

            _lastMessage = messageTransform as RectTransform;
            var canvasGroup = go.GetComponent<CanvasGroup>() ?? go.AddComponent<CanvasGroup>();

            canvasGroup.alpha = 1f;
            messageTransform.localPosition = Vector3.zero;
            messageTransform.localScale = Vector3.one;

            // Animation Sequence
            _startTime = Time.unscaledTime;
            var sequence = DOTween.Sequence();
            sequence.SetUpdate(true);
            sequence.Append(messageTransform.DOLocalMove(new Vector3(0f, Height, 0f), _duration).SetEase(Ease.OutCubic));
            sequence.Join(canvasGroup.DOFade(0f, _duration).SetEase(Ease.InCubic));
            sequence.OnComplete(() => { _lastMessage = null; });
            yield return sequence.WaitForCompletion();
        }

        private bool _IsOverlap(RectTransform newTransform)
        {
            // check if the top of the new transform overlaps with the bottom of the last message transform
            if (!_lastMessage || !_lastMessage.gameObject.activeInHierarchy) return false;
            var newTop = newTransform.position.y + newTransform.rect.height / 2;
            var lastBottom = _lastMessage.position.y - _lastMessage.rect.height / 2;
            return newTop > lastBottom;
        }
    }
}