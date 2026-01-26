#if AUDIO_MOB
using Audiomob;
using Shared.Service.AudioMob;
using Shared.Service.AudioMob.Internal;
#endif
using System.Collections;
using Shared.Core.Async;
using Shared.Service.AudioAds;
using Shared.Service.AudioAds.Internal;
using Shared.Service.Tracking;
using Shared.Tracking.Models.Ads;
using Shared.Utilities;
using Shared.Utils;
using Shared.View.AudioAds;
using UnityEngine;

namespace Shared.View.AudioMob
{
    [DisallowMultipleComponent]
    public class AudioMobAd : MonoBehaviour, ISharedAudioAd, ISharedUtility, ISharedLogTag
    {
        public string LogTag => SharedLogTag.AudioAdsNAudioMob;

        [SerializeField] private GameObject audioMobPlugin;
        [SerializeField] private RectTransform banner;
        [SerializeField] private Camera camera;

        public AudioAdSource AudioAdSource => AudioAdSource.AudioMob;
        public bool IsReadyToPlay => true;

        private IAsyncOperation _playOperation;
        private bool IsPlaying => _playOperation != null;
        
        private string _placementName = "unknown";

#if AUDIO_MOB
        private AudiomobPlugin _audioMobPluginScript;
        private AudiomobPlugin AudioMobPluginScript
        {
            get
            {
                if (_audioMobPluginScript != null) return _audioMobPluginScript;
                _audioMobPluginScript = audioMobPlugin.GetComponent<AudiomobPlugin>();
                return _audioMobPluginScript;
            }
        }

        private void Awake()
        {
            this.LogInfo(nameof(name), name);
            _AddListeners();
            AudioAdRegistry.Register(this);
        }

        private void OnDestroy()
        {
            this.LogInfo(nameof(name), name);
            _RemoveListeners();
            AudioAdRegistry.Remove(this);
        }
#endif

        public void MoveTo(AudioAdPlacement placement)
        {
            banner.position = placement.transform.position;
            _placementName = placement.Name;
        }

        public void Stop()
        {
            this.LogInfo();
#if AUDIO_MOB
            AudioMobPluginScript.StopAd(_placementName);
#endif
        }

        public void Pause()
        {
            this.LogInfo(nameof(_placementName), _placementName, nameof(IsPlaying), IsPlaying);
#if AUDIO_MOB
            if (IsPlaying) AudioMobPluginScript.PauseAd(_placementName);
#endif
        }

        public void Resume()
        {
            this.LogInfo(nameof(_placementName), _placementName, nameof(IsPlaying), IsPlaying);
#if AUDIO_MOB
            if (IsPlaying) AudioMobPluginScript.ResumeAd(_placementName);
#endif
        }

        public IAsyncOperation Play(AudioAdPlacement placement)
        {
            this.LogInfo();
            MoveTo(placement);
            if (_playOperation != null) return _playOperation;
            _playOperation = new SharedAsyncOperation();
#if UNITY_EDITOR && AUDIO_MOB
            banner.SetActive(true);
            StartCoroutine(CRLoadAd());
            IEnumerator CRLoadAd()
            {
                yield return new WaitForSeconds(60);
                _playOperation.Success();
                banner.SetActive(false);
            }
#elif AUDIO_MOB
            // 15:30:18.853  I  [AudioAds,AudioMob] AudioMobAd->Play: {}
            // 15:30:18.858  W  <b>[AudioMob]</b> Can't request and play ad, one is already requesting or playing.
            // Do nó không callback nếu ad bị error, nên mình cần chủ động remove _playOperation
            this.LogInfo("AudioMob v10 - Playing ad with default ad unit");
            this.Track(AdLoadParams.AudioAdLoadParams());
            // AudioMob v10: Use PlayAd with ad unit ID instead of RequestAndPlayAd
            AudioMobPlugin.Instance.PlayAd(AudiomobPlugin.AdUnits.SkippableRectangle);
#endif
            return _playOperation;
        }

#if AUDIO_MOB
        /// <summary>
        /// This callback is invoked when the ad request has been completed.
        /// DEPRECATED in v10 - Use OnAdPlaybackStatusChanged or OnAdFailed instead
        /// </summary>
        private void _OnAdRequestCompleted(AdRequestResult adRequestResult, IAudioAd audioAd)
        {
            this.LogInfo(nameof(_placementName), _placementName, nameof(adRequestResult), adRequestResult.ToString(), nameof(audioAd), audioAd.ToDict());
            if (adRequestResult != AdRequestResult.Finished)
            {
                this.LogError(nameof(adRequestResult), adRequestResult.ToString());
                _playOperation?.Fail($"adRequestResult={adRequestResult.ToString()}");
                _playOperation = null;
            }

            // AudioMob v10: This event is deprecated, using v10 unified events instead
            // AudioMobEvents.OnAdRequestCompletedEvent.Invoke(_placementName, adRequestResult, audioAd);
        }

        /// <summary>
        /// This callback is invoked when the ad begins to play.
        /// DEPRECATED in v10 - Use OnAdPlaybackStatusChanged instead
        /// </summary>
        private void _OnAdPlaybackStarted(IAudioAd audioAd)
        {
            this.LogInfo(nameof(_placementName), _placementName, nameof(audioAd), audioAd.ToDict());
            gameObject.SetActive(true);
            // AudioMob v10: This event is deprecated, using OnAdPlaybackStatusChanged instead
            // AudioMobEvents.OnAdPlaybackStartedEvent.Invoke(_placementName, audioAd);
        }

        /// <summary>
        /// This callback is invoked when the ad finishes playing.
        /// DEPRECATED in v10 - Use OnAdPlaybackStatusChanged instead
        /// </summary>
        private void _OnAdPlaybackCompleted(AdPlaybackStatus adPlaybackResult)
        {
            this.LogInfo(nameof(_placementName), _placementName, nameof(adPlaybackResult), adPlaybackResult.ToString());
            _playOperation?.Success();
            _playOperation = null;
            // AudioMob v10: This event is deprecated, using OnAdPlaybackStatusChanged instead
            // AudioMobEvents.OnAdPlaybackCompletedEvent.Invoke(_placementName, adPlaybackResult);
        }

        private void _OnSkipButtonAvailable()
        {
            this.LogInfo(nameof(_placementName), _placementName);
            // AudioMob v10: This event is deprecated
            // AudioMobEvents.OnSkipButtonAvailableEvent.Invoke(_placementName);
        }
        
        private void _OnAdPlaybackStatusChanged(AdSequence adSequence, AdPlaybackStatus adPlaybackStatus)
        {
            this.LogInfo(nameof(_placementName), _placementName, nameof(adSequence), adSequence.ToString(), nameof(adPlaybackStatus), adPlaybackStatus.ToString());
            
            // Handle v10 unified status changes
            switch (adPlaybackStatus)
            {
                case AdPlaybackStatus.Started:
                    // Equivalent to old _OnAdPlaybackStarted
                    gameObject.SetActive(true);
                    break;
                    
                case AdPlaybackStatus.Finished:
                case AdPlaybackStatus.Skipped:
                case AdPlaybackStatus.Stopped:
                case AdPlaybackStatus.Canceled:
                    // Equivalent to old _OnAdPlaybackCompleted
                    _playOperation?.Success();
                    _playOperation = null;
                    break;
                    
                case AdPlaybackStatus.PlaybackFailed:
                case AdPlaybackStatus.RequestFailed:
                    // Handle failures
                    _playOperation?.Fail($"adPlaybackStatus={adPlaybackStatus.ToString()}");
                    _playOperation = null;
                    break;
            }
            
            AudioMobEvents.OnAdPlaybackStatusChangedEvent.Invoke(_placementName, adSequence, adPlaybackStatus);
        }
        
        private void _OnAdFailed(string adUnitId, AdFailureReason adFailureReason)
        {
            this.LogInfo(nameof(_placementName), _placementName, nameof(adUnitId), adUnitId, nameof(adFailureReason), adFailureReason.ToString());
            _playOperation?.Fail($"adFailureReason={adFailureReason.ToString()}");
            _playOperation = null;
            AudioMobEvents.OnAdFailedEvent.Invoke(_placementName, null, adFailureReason.ToString());
        }
        
        private void _OnAdPaid(IAudioAd audioAd)
        {
            this.LogInfo(nameof(_placementName), _placementName, nameof(audioAd), audioAd.ToDict());
            AudioMobEvents.OnAdPaidEvent.Invoke(_placementName, null, audioAd);
        }
        
        private void _OnAdClicked(string adUnitId)
        {
            this.LogInfo(nameof(_placementName), _placementName, nameof(adUnitId), adUnitId);
            AudioMobEvents.OnAdClickedEvent.Invoke(_placementName, null);
        }
#endif

#if AUDIO_MOB
    
        private void _AddListeners()
        {
            // AudioMob v9 callbacks - deprecated in v10
            // AudioMobPluginScript.OnAdRequestCompleted += _OnAdRequestCompleted;
            // AudioMobPluginScript.OnAdPlaybackStarted += _OnAdPlaybackStarted;
            // AudioMobPluginScript.OnAdPlaybackCompleted += _OnAdPlaybackCompleted;
            // AudioMobPluginScript.OnSkipButtonAvailable += _OnSkipButtonAvailable;

            // AudioMob v10 unified callbacks
            AudioMobPluginScript.OnAdPlaybackStatusChanged += _OnAdPlaybackStatusChanged;
            AudioMobPluginScript.OnAdFailed += _OnAdFailed;
            AudioMobPluginScript.OnAdPaid += _OnAdPaid;
            AudioMobPluginScript.OnAdClicked += _OnAdClicked;
        }

        private void _RemoveListeners()
        {
            // AudioMob v9 callbacks - deprecated in v10
            // AudioMobPluginScript.OnAdRequestCompleted -= _OnAdRequestCompleted;
            // AudioMobPluginScript.OnAdPlaybackStarted -= _OnAdPlaybackStarted;
            // AudioMobPluginScript.OnAdPlaybackCompleted -= _OnAdPlaybackCompleted;
            // AudioMobPluginScript.OnSkipButtonAvailable -= _OnSkipButtonAvailable;
            
            // AudioMob v10 unified callbacks
            if (AudioMobPluginScript != null)
            {
                AudioMobPluginScript.OnAdPlaybackStatusChanged -= _OnAdPlaybackStatusChanged;
                AudioMobPluginScript.OnAdFailed -= _OnAdFailed;
                AudioMobPluginScript.OnAdPaid -= _OnAdPaid;
                AudioMobPluginScript.OnAdClicked -= _OnAdClicked;
            }
        }
#endif
    }
}