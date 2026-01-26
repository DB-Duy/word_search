using Shared.Core.IoC;
using Shared.Utils;
using UnityEngine;

namespace Shared.View.InPlayAds
{
    public abstract class AbstractInPlayAd : IoCMonoBehavior, IInPlayAd
    {
        private string _className;
        public string ClassName => _className ??= GetType().Name;
        
        public abstract bool IsReady { get; protected set; }
        public string ForPlacementName { get; set; }
        [SerializeField] private GameObject rootObject;
        public Transform MyTransform => rootObject == null ? transform : rootObject.transform;

        public void ResetReadyState()
        {
            IsReady = false;
        }
        
        public void SetActive(bool active)
        {
            this.LogInfo(SharedLogTag.InPlayAds, "clazz", GetType().Name, nameof(active), active);
            if (!active) IsReady = false;
            if (rootObject != null)
            {
                rootObject.SetActive(active);
                return;
            }
            gameObject.SetActive(active);
        }
    }
}