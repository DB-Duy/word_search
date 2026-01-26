using System.Collections;
using DG.Tweening;
using UnityEngine;

namespace Shared.Core.View.Dialog
{
    public class DialogShowPresenter : ISharedUtility, IDialogShowPresenter
    {
        private readonly float _duration;

        public DialogShowPresenter(float duration = 0.5f)
        {
            _duration = duration;
        }

        public IEnumerator Present(MonoBehaviour o)
        {
            var template = DialogTemplate.Create(o);
            if (template == null) yield break;
            template.DimBackground.alpha = 0f;
            template.Content.localScale = Vector3.zero;
            var sequence = DOTween.Sequence();
            sequence.Append(template.DimBackground.DOFade(1f, _duration / 2));
            sequence.Join(template.Content.DOScale(1f, _duration).SetEase(Ease.OutBack));
            sequence.Play();
        }
    }
}