using Shared.Core.IoC;
using Shared.Service.Ads;
using Shared.Utilities;
using Shared.Utils;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Shared.View.Ad
{
    [RequireComponent(typeof(RectTransform))]
    public class BannerAdsPlacement : IoCMonoBehavior, ISharedUtility, ISharedLogTag
    {
        public string LogTag => SharedLogTag.AdNBanner;

        [Inject] private IBannerAdsPlacementService _placementService;

        [SerializeField] private BannerType top = BannerType.None;
        [SerializeField] private BannerType bottom = BannerType.None;

        [SerializeField] private RectTransform rectTransform;
        [SerializeField] private CanvasScaler canvasScaler;

        protected override void Awake()
        {
            base.Awake();
            this.LogInfo(nameof(name), name);
            _placementService.Register(this);
            if (rectTransform == null) rectTransform = GetComponent<RectTransform>();
            if (canvasScaler == null) canvasScaler = GetComponentInParent<CanvasScaler>();

            if (canvasScaler == null) this.LogError(nameof(canvasScaler), canvasScaler);
        }

        private void OnDestroy() => _placementService.Remove(this);

        private void OnEnable()
        {
            var e = _placementService.IsVisible();
            UpdateView(e);
        }

        public void UpdateView(bool isVisible)
        {
            _UpdateTop(isVisible);
            _UpdateBottom(isVisible);
        }

        private void _UpdateTop(bool isVisible)
        {
            var offset = !isVisible ? 0f : canvasScaler.ConvertBannerTypeToCanvasUnits(top);
            rectTransform.offsetMax = new Vector2(rectTransform.offsetMax.x, -offset);
            this.LogInfo(nameof(isVisible), isVisible);
        }

        private void _UpdateBottom(bool isVisible)
        {
            var offset = !isVisible ? 0f : canvasScaler.ConvertBannerTypeToCanvasUnits(bottom);
            rectTransform.offsetMin = new Vector2(rectTransform.offsetMin.x, offset);
            this.LogInfo(nameof(isVisible), isVisible);
        }

#if UNITY_EDITOR
        public override void GUIEditor()
        {
            GUILayout.Label("Editor Quick Actions");
            base.GUIEditor();

            if (GUILayout.Button("Print Anchor"))
            {
                SharedLogger.LogJson($"GUIEditor", "anchorMin", rectTransform.anchorMin.ToString(), "anchorMax",
                    rectTransform.anchorMax.ToString());
            }

            if (GUILayout.Button("Print Offset"))
            {
                SharedLogger.LogJson($"GUIEditor", "offsetMin", rectTransform.offsetMin.ToString(), "offsetMax",
                    rectTransform.offsetMax.ToString());
            }

            if (GUILayout.Button("Show"))
            {
                UpdateView(true);
                // _placementService.IsVisible = true;
            }

            if (GUILayout.Button("Hide"))
            {
                UpdateView(false);
                // BannerAdsPlacementExtensions.IsVisible = false;
            }
        }
#endif
    }
}