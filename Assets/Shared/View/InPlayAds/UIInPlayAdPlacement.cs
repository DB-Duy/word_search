using System;
using System.Collections;
using Shared.Core.IoC;
using Shared.Service.InPlayAds;
using Shared.Utils;
using UnityEngine;

namespace Shared.View.InPlayAds
{
    [DisallowMultipleComponent]
    public class UIInPlayAdPlacement : IoCMonoBehavior, ISharedUtility
    {
        [SerializeField] private RectTransform[] overlapChecks;
        [SerializeField] private string placementName;
        public string PlacementName => placementName;

        public bool? IsOverLapOtherUis { get; private set; } = null;

        private void OnEnable()
        {
            gameObject.GetComponent<RectTransform>().RemoveRedImage();
            StartCoroutine(_ValidateByOverlap());
        }

        private void OnDisable()
        {
            InPlayAdRegistry.Remove(this);
        }
        
        private IEnumerator _ValidateByOverlap()
        {
            // Delay 2 frames
            yield return null;
            yield return null;
            
            var me = GetComponent<RectTransform>();
            IsOverLapOtherUis = me.IsOnScreenOverlapOneOf(overlapChecks);
            if (IsOverLapOtherUis == true)
            {
                me.RemoveRedImage();
                yield break;
            }
#if LOG_INFO
            me.AddRedImage();
#endif
            this.LogInfo(SharedLogTag.InPlayAds, nameof(IsOverLapOtherUis), IsOverLapOtherUis);
            InPlayAdRegistry.Register(this);
        }
    }
}