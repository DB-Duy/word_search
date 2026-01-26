using System.Collections;
using Shared.Core.IoC;
using Shared.Service.Tracking;
using Shared.Service.Tracking.Common;
using Shared.Service.Ump;
using Shared.Tracking.Models.Game;
using Shared.Utils;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Shared.View.Ump
{
    [DisallowMultipleComponent]
    public class EditConsentView : IoCMonoBehavior, ISharedUtility
    {
        [SerializeField] private GameObject rootObject;
        [SerializeField] private Button editButton;

        [Inject(Optional = true)] private IUmpService _umpService;

        private void Start()
        {
            editButton.onClick.AddListener(() => StartCoroutine(_EditConsent()));
        }

        private void OnEnable()
        {
            editButton.interactable = _umpService != null && _umpService.CanEdit();
        }

        private IEnumerator _EditConsent()
        {
#if USING_UMP
            this.LogInfo(SharedLogTag.Ump);
            var o = _umpService.Edit();
            yield return new WaitUntil(() => o.IsComplete);
            if (o.IsSuccess)
                this.Track(new GameScreenParams(TrackingScreen.Ump));
#endif
            yield return null;
        }
    }
}