using System.Collections;
#if ODEEO_AUDIO
using Odeeo;
using Odeeo.Data;
#endif
using Shared.Core.Async;
using Shared.Core.IoC;
using Shared.Entity.Config;
using Shared.Service.AudioAds;
using Shared.Service.AudioAds.Internal;
using Shared.Service.Odeeo;
using Shared.Service.Odeeo.Internal;
using Shared.Service.Tracking;
using Shared.Tracking.Models.Ads;
using Shared.Utilities;
using Shared.Utils;
using Shared.View.AudioAds;
using UnityEngine;
using Zenject;

namespace Shared.View.Odeeo
{
    [DisallowMultipleComponent]
    public class OdeeoAudioAd : IoCMonoBehavior, ISharedAudioAd, ISharedUtility, ISharedLogTag
    {
#if ODEEO_AUDIO
        [Inject] private IOdeeoService _odeeoService;
        private IOdeeoAdUnitCallbacks _odeeoAdListener;
#endif
        
        [Inject] private IConfig _config;

        private string _placementName;
        [SerializeField] private string odeeoPlacementId;

        public AudioAdSource AudioAdSource => AudioAdSource.Odeoo;
        public bool IsReadyToPlay { get; private set; }
        private IAsyncOperation _playOperation;
        
#if ODEEO_AUDIO
        private void Start()
        {
            odeeoPlacementId = string.IsNullOrEmpty(odeeoPlacementId) ? _config.OdeeoIconId : odeeoPlacementId;
            this.LogInfo(nameof(name), name, nameof(odeeoPlacementId), odeeoPlacementId);
            AudioAdRegistry.Register(this);
            if (Application.isEditor) return;
            StartCoroutine(_CreateAdUnitIfNotExisted());
        }

        private void OnDestroy() => AudioAdRegistry.Remove(this);
#endif
        
        public IAsyncOperation Play(AudioAdPlacement placement)
        {
            if (_playOperation != null) return _playOperation;
            this.LogInfo(nameof(name), name);
            MoveTo(placement);
            _playOperation = new SharedAsyncOperation();
#if UNITY_EDITOR
            StartCoroutine(CRLoadAd());
            IEnumerator CRLoadAd()
            {
                yield return new WaitForSeconds(60);
                _playOperation.Success();
                _playOperation = null;
            }
#elif ODEEO_AUDIO
            this.Track(IsReadyToPlay ? AdReadyParams.Audio(placement.Name) : AdNotReadyParams.Audio(placement.Name));
            OdeeoAdManager.ShowAd(odeeoPlacementId);
#endif
            return _playOperation;
        }

        public void MoveTo(AudioAdPlacement placement)
        {
            _placementName = placement.Name;
#if ODEEO_AUDIO
            this.LogInfo(nameof(_placementName), _placementName);
            var canvas = placement.GetComponentInParent<Canvas>(true);
            if (canvas == null) this.LogError(nameof(_placementName), _placementName, "canvas", "null");
            OdeeoAdManager.LinkIconToRectTransform(odeeoPlacementId, OdeeoSdk.IconPosition.Centered, (RectTransform)placement.transform, canvas);
#endif
        }

        public void Stop()
        {
            this.LogInfo(nameof(odeeoPlacementId), odeeoPlacementId);
#if ODEEO_AUDIO
            OdeeoAdManager.RemoveAd(odeeoPlacementId);
#endif
        }

        public void Pause()
        {
            this.LogInfo(nameof(odeeoPlacementId), odeeoPlacementId);
#if ODEEO_AUDIO
            OdeeoAdManager.RemoveAd(odeeoPlacementId);
#endif
        }

        public void Resume()
        {
            this.LogWarning("result", "not implemented");
        }

#if ODEEO_AUDIO
        private IEnumerator _CreateAdUnitIfNotExisted()
        {
            this.LogInfo();
            if (Application.isEditor) yield break;
            if (_odeeoAdListener != null)
            {
                this.LogError(nameof(_odeeoAdListener), "not null");
                yield break;
            }

            while (!_odeeoService.IsInitialized) yield return null;

            this.LogInfo(nameof(name), name, nameof(odeeoPlacementId), odeeoPlacementId, nameof(_config.OdeeoIconId), _config.OdeeoIconId);

            OdeeoAdManager.CreateAudioIconAd(odeeoPlacementId);
            _odeeoAdListener = OdeeoAdManager.AdUnitCallbacks(odeeoPlacementId);
            _odeeoAdListener.OnAvailabilityChanged += _OnAvailabilityChanged;
            _odeeoAdListener.OnShow += _OnShow;
            _odeeoAdListener.OnShowFailed += _OnShowFailed;
            _odeeoAdListener.OnClose += _OnClose;
            _odeeoAdListener.OnClick += _OnClick;
            _odeeoAdListener.OnPause += _OnPause;
            _odeeoAdListener.OnResume += _OnResume;
            _odeeoAdListener.OnMute += _OnMute;
            _odeeoAdListener.OnImpression += _OnImpression;
        }

        // -------------------------------------------------------------------------------------------------------------
        // Callbacks
        // -------------------------------------------------------------------------------------------------------------
        private void _OnAvailabilityChanged(bool flag, OdeeoAdData data)
        {
            this.LogInfo(nameof(_placementName), _placementName, nameof(odeeoPlacementId), odeeoPlacementId, nameof(flag), flag, nameof(data), data.ToDict());
            IsReadyToPlay = flag;
            OdeeoEvents.OnPlacementAvailabilityChangedEvent.Invoke(_placementName, flag);
        }

        private void _OnShow()
        {
            this.LogInfo(nameof(_placementName), _placementName, nameof(odeeoPlacementId), odeeoPlacementId);
            OdeeoEvents.OnPlacementShowEvent.Invoke(_placementName);
        }

        private void _OnShowFailed(string placementId, OdeeoAdUnit.ErrorShowReason reason, string description)
        {
            this.LogInfo(nameof(_placementName), _placementName, nameof(placementId), placementId, nameof(reason),
                reason.ToString(), nameof(description), description);
            _playOperation?.Fail(reason.ToString());
            _playOperation = null;
            OdeeoEvents.OnPlacementShowFailedEvent.Invoke(_placementName, reason.ToString(), description);
        }

        private void _OnClose(OdeeoAdUnit.CloseReason reason)
        {
            this.LogInfo(nameof(_placementName), _placementName, nameof(odeeoPlacementId), odeeoPlacementId,
                nameof(reason), reason.ToString());
            _playOperation?.Success();
            _playOperation = null;
            OdeeoEvents.OnPlacementCloseEvent.Invoke(_placementName);
        }

        private void _OnClick()
        {
            this.LogInfo(nameof(_placementName), _placementName, nameof(odeeoPlacementId), odeeoPlacementId);
            OdeeoEvents.OnPlacementClickEvent.Invoke(_placementName);
        }

        private void _OnPause(OdeeoAdUnit.StateChangeReason reason)
        {
            this.LogInfo(nameof(_placementName), _placementName, nameof(odeeoPlacementId), odeeoPlacementId,
                nameof(reason), reason.ToString());
            OdeeoEvents.OnPlacementPauseEvent.Invoke(_placementName);
        }

        private void _OnResume(OdeeoAdUnit.StateChangeReason reason)
        {
            this.LogInfo(nameof(_placementName), _placementName, nameof(odeeoPlacementId), odeeoPlacementId,
                nameof(reason), reason.ToString());
            OdeeoEvents.OnPlacementResumeEvent.Invoke(_placementName);
        }

        private void _OnMute(bool isMuted)
        {
            this.LogInfo(nameof(_placementName), _placementName, nameof(odeeoPlacementId), odeeoPlacementId);
            OdeeoEvents.OnPlacementMuteEvent.Invoke(_placementName);
        }

        private void _OnImpression(OdeeoImpressionData data)
        {
            this.LogInfo(nameof(_placementName), _placementName, nameof(odeeoPlacementId), odeeoPlacementId,
                nameof(data), data.ToDict());
            OdeeoEvents.OnPlacementImpressionEvent.Invoke(_placementName, data);
        }

#endif
        public string LogTag => SharedLogTag.AudioAdsNOdeoo;
    }
}