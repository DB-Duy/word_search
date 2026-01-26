using System.Collections;
using Shared.Core.Async;
using Shared.Core.IoC;
using Shared.Service.AudioAds;
using Shared.Service.AudioAds.Internal;
using Shared.Service.Gadsme;
using Shared.View.AudioAds;
using UnityEngine;
using Zenject;

namespace Shared.View.Gadsme
{
    [DisallowMultipleComponent]
    public class GadsmeAudioAd300x250 : IoCMonoBehavior, ISharedAudioAd, ISharedUtility
    {
        [SerializeField] private GameObject banner;
        [Inject] private IGadsmeService _gadsmeService;

        private string _placementName;
        public AudioAdSource AudioAdSource => AudioAdSource.Gadsme;
        public bool IsReadyToPlay => _gadsmeService.IsAudioAdAvailable;

#if GADSME
        private void Start()
        {
            AudioAdRegistry.Register(this);
        }

        private void OnDestroy()
        {
            AudioAdRegistry.Remove(this);
        }
#endif


        public IAsyncOperation Play(AudioAdPlacement placement)
        {
            MoveTo(placement);
#if UNITY_EDITOR
            var playOperation = new SharedAsyncOperation();
            banner.SetActive(true);
            StartCoroutine(CRLoadAd());
            return playOperation;
            IEnumerator CRLoadAd()
            {
                yield return new WaitForSeconds(60);
                playOperation.Success();
                gameObject.SetActive(false);
            }
#elif GADSME
            return _gadsmeService.PlayAudioAd(_placementName);
#else
            return new SharedAsyncOperation().Success();
#endif
        }

        public void MoveTo(AudioAdPlacement placement)
        {
            _placementName = placement.Name;
            banner.transform.position = placement.transform.position;
        }

        public void Stop()
        {
            _gadsmeService.Stop();
        }

        public void Pause()
        {
            _gadsmeService.Stop();
        }

        public void Resume()
        {
        }
    }
}