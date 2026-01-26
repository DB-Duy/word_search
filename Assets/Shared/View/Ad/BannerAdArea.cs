using Shared.Utilities.SharedBehaviour;
using Shared.Utils;
using UnityEngine;

namespace Shared.View.Ad
{
    [DisallowMultipleComponent]
    public class BannerAdArea : SharedMonoBehaviour
    {
        [SerializeField] private Camera camera;
        
        private void OnEnable()
        {
            if (camera == null)
            {
                var canvas =  GetComponentInParent<Canvas>();
                if (canvas != null)
                    camera = canvas.worldCamera;
            }
            
            // _adHeightInPixels = IosScaleFactorPlugin.ConvertDpToPx(_adHeight);
#if UNITY_ANDROID
            var heightPx = SharedUtilities.ConvertDpToPx(54);
#else
            // BannerAdArea->OnEnable: {"scaleFactor":3.0,"nativeScaleFactor":2.60869575,"heightPx":162.0}
            var scaleFactor = IosScaleFactorPlugin.GetScaleFactor();
            var nativeScaleFactor = IosScaleFactorPlugin.GetNativeScaleFactor();
            var maxScaleFactor = Mathf.Max(scaleFactor, nativeScaleFactor);
            var heightPx = 58 * maxScaleFactor;
            this.LogInfo(nameof(scaleFactor), scaleFactor, nameof(nativeScaleFactor), nativeScaleFactor, nameof(heightPx), heightPx);
#endif
            var rect = GetComponent<RectTransform>();
            RectTransformUtility.ScreenPointToLocalPointInRectangle(rect, new Vector2(0, 0), camera, out var aPoint);
            RectTransformUtility.ScreenPointToLocalPointInRectangle(rect, new Vector2(0, heightPx), camera, out var bPoint);
            var size = new Vector2(rect.sizeDelta.x, bPoint.y - aPoint.y);
            rect.sizeDelta = size;
            this.LogInfo(nameof(rect.sizeDelta), rect.sizeDelta.ToString());
#if LOG_INFO
            rect.AddRedImage();
#else
            rect.RemoveRedImage();
#endif
        }

#if UNITY_EDITOR
        public override void GUIEditor()
        {
            if (GUILayout.Button("Set 999"))
            {
                var rect = GetComponent<RectTransform>();
                rect.sizeDelta = new Vector2(rect.sizeDelta.x, 999);
            }

        }
#endif
    }
}