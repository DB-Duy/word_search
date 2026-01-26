using Shared.Core.IoC;
using Shared.Service.AudioAds;
using Shared.Service.AudioAds.Internal;
using UnityEngine;
using Zenject;

namespace Shared.View.AudioAds
{
    [DisallowMultipleComponent]
    public class AudioAdPlacement : IoCMonoBehavior
    {
        [SerializeField] private string placementName;

        [Inject] private IAudioAdsService _audioAdsService;

        public string Name => placementName;


        private void Start()
        {
#if LOG_INFO
            gameObject.GetComponent<RectTransform>().AddRedImage();   
#else
            gameObject.GetComponent<RectTransform>().RemoveRedImage();
#endif
        }


        private void OnEnable()
        {
            AudioAdRegistry.Register(this);
        }

        private void OnDisable()
        {
            AudioAdRegistry.Remove(this);
        }
        
        public Vector3 ScreenPosition => Camera.main.WorldToScreenPoint(transform.position);
    }
}