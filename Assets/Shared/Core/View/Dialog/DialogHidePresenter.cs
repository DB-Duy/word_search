using System.Collections;
using DG.Tweening;
using Shared.Core.Presenter;
using UnityEngine;

namespace Shared.Core.View.Dialog
{
    public class DialogHidePresenter : ISharedUtility, IDialogHidePresenter
    {
        private readonly float _duration;
        
        public DialogHidePresenter(float duration = 1f)
        {
            _duration = duration;
        }

        public IEnumerator Present(MonoBehaviour o)
        {
            var template = DialogTemplate.Create(o);
            if (template == null) yield break;
            // Animation Sequence
            var sequence = DOTween.Sequence();
            sequence.Append(template.Content.DOScale(0f, _duration / 2).SetEase(Ease.InBack));
            sequence.Join(template.DimBackground.DOFade(0f, _duration / 2));
            var isComplete = false;
            sequence.OnComplete(() => isComplete = true);
            sequence.Play();
            while (!isComplete) yield return null;
        }
    }
}