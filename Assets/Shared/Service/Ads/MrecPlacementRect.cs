using System.Collections;
using Shared.Core.IoC;
using Shared.Entity.Config;
using Shared.Service.Ads.Common;
using Shared.Utils;
using UnityEngine;
using Zenject;

namespace Shared.Service.Ads
{
    [DisallowMultipleComponent]
    public class MrecPlacementRect : IoCMonoBehavior, ISharedUtility
    {
        [SerializeField] private string placementName;
        [SerializeField] private bool automationMode = true;
        [SerializeField] private Camera camera;
        
        [Inject(Optional = true)] private IMrecAd _mrecAd;
        [Inject] private IConfig config;
        private IAdPlacement _p;
        public IAdPlacement Placement => _p ??= new AdPlacement(placementName);
        
        private void OnEnable()
        {
            if (camera == null)
            {
                var canvas =  GetComponentInParent<Canvas>();
                if (canvas != null)
                    camera = canvas.worldCamera;
            }

            if (automationMode)
                StartCoroutine(_RegisterAndShow());
            
            var widthPx = SharedUtilities.ConvertDpToPx(310);
            var heightPx = SharedUtilities.ConvertDpToPx(260);
            var rect = GetComponent<RectTransform>();
            RectTransformUtility.ScreenPointToLocalPointInRectangle(rect, new Vector2(0, 0), camera, out var aPoint);
            RectTransformUtility.ScreenPointToLocalPointInRectangle(rect, new Vector2(widthPx, heightPx), camera, out var bPoint);
            var size = new Vector2(bPoint.x - aPoint.x, bPoint.y - aPoint.y);
            rect.sizeDelta = size;
            this.LogInfo(SharedLogTag.AdNLevelPlayNMrec, nameof(rect.sizeDelta), rect.sizeDelta.ToString());
#if LOG_INFO
            rect.AddRedImage();
#else
            rect.RemoveRedImage();
#endif
        }

        private void OnDisable()
        {
            _mrecAd?.Hide(Placement);
        }

        public void ShowAdByManual()
        {
            StartCoroutine(_RegisterAndShow());
        }
        
        public void HideAdByManual()
        {
            _mrecAd?.Hide(Placement);
        }

        private IEnumerator _RegisterAndShow()
        {
            while (!AdFlag.IsMrecInitialized) yield return null;
            var screenPoint = ((RectTransform)transform).GetDpPosition(camera, 300f, 250f);
            this.LogInfo(SharedLogTag.AdNLevelPlayNMrec, nameof(screenPoint), screenPoint.ToString());
            var c = config;
            if (c.FixedDpi > 0)
            {
                if (Screen.dpi > c.FixedDpi)
                    screenPoint *= Screen.dpi / c.FixedDpi;
            }
            _mrecAd?.Register(Placement, screenPoint);
            _mrecAd?.Show(Placement);
        }
    }
}