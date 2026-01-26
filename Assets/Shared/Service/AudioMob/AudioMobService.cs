#if AUDIO_MOB
using System;
using Audiomob;
using Shared.Core.Async;
using Shared.Core.IoC;
using Shared.Service.AudioMob.Internal;
using Shared.Service.Tracking;      
using Shared.Tracking.Models.Ads;
using Shared.Utils;
using Zenject;
using Shared.Service.Audio;
using Shared.Service.Audio.Internal;
using Shared.Service.Tracking.TrackingEvent.Ads;
using Shared.Utilities;

namespace Shared.Service.AudioMob
{
    /// <summary>
    /// 2023-09-06 12:11:24.937 28498-28641/com.indiez.nonogram I/Unity: AuAd AudioMobAdManager->Show: gameplay
    /// 2023-09-06 12:11:27.751 28498-28641/com.indiez.nonogram I/Unity: AuAd AudioMobAdManager->_OnAdRequestCompleted: Finished - {"AdFormat":"Skippable","Duration":30.302,"EstimatedCPM":0.0,"EstimatedRevenue":0.0,"ExpiryTime":"2023-09-06T12:31:25.446+07:00"}
    /// 2023 - 09 - 06 12:11:27.760 28498 - 28641 / com.indiez.nonogram I / Unity: AuAd AudioMobAdManager->_OnAdPlaybackStarted:: {"AdFormat":"Skippable","Duration":30.302,"EstimatedCPM":0.0,"EstimatedRevenue":0.0,"ExpiryTime":"2023-09-06T12:31:25.446+07:00"}
    /// 2023 - 09 - 06 12:11:37.692 28498 - 28641 / com.indiez.nonogram I / Unity: AuAd AudioMobAdManager->_OnSkipButtonAvailable
    /// 2023-09-06 12:12:08.871 28498 - 28641 / com.indiez.nonogram I / Unity: AuAd AudioMobAdManager->_OnAdPlaybackCompleted: Finished
    /// </summary>
    [Service]
    public class AudioMobService : IAudioMobService, ISharedUtility, ISharedLogTag
    {
        public string LogTag => SharedLogTag.AudioAdsNAudioMob;

        [Inject] private IAudioService _audioService;
        
        public bool IsInitialized { get; private set; } = false;
        private IAsyncOperation _initOperation;

        private readonly MuteRequest _muteRequest = new("AudioMobService");
        
        public IAsyncOperation Initialize()
        {
            if (_initOperation != null) return _initOperation;
            _initOperation = new SharedAsyncOperation().Success();
            IsInitialized = true;
            
            // AudioMobEvents.OnAdRequestCompletedEvent += _OnAdRequestCompleted;
            // AudioMobEvents.OnAdPlaybackStartedEvent += _OnAdPlaybackStarted;
            // AudioMobEvents.OnAdPlaybackCompletedEvent += _OnAdPlaybackCompleted;
            // AudioMobEvents.OnSkipButtonAvailableEvent += _OnSkipButtonAvailable;
            
            // Audiomob Unity Plugin v10 Upgrade
            AudioMobEvents.OnAdPlaybackStatusChangedEvent += _OnAdPlaybackStatusChangedEvent;
            AudioMobEvents.OnAdPaidEvent += _OnAdPaid;
            AudioMobEvents.OnAdClickedEvent += _OnAdClicked;
            AudioMobEvents.OnAdFailedEvent += _OnAdFailed;

            // AudioMobPluginInitialization.InitializePlugin();
            return _initOperation;
        }

        // -------------------------------------------------------------------------------------------------------------
        // Callbacks
        // -------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// This callback is invoked when the ad request has been completed.
        /// </summary>
        private void _OnAdRequestCompleted(string placement, AdRequestResult adRequestResult, IAudioAd audioAd)
        {
            this.LogInfo(nameof(placement), placement, nameof(adRequestResult), adRequestResult.ToString(), nameof(audioAd), audioAd.ToDict());
            switch (adRequestResult)
            {
                case AdRequestResult.Finished:
                    /* Write code here to:
                        - Get info about the audio via the IAudioAd argument, such as audio length or expiry time
                        - Check the time that the ad has taken to load. */
                    this.Track(AdLoadSucceededParams.Audio());
                    break;
                case AdRequestResult.NoAdAvailable:
                case AdRequestResult.SkippableRequestVolumeNotAudible:
                case AdRequestResult.Failed:
                case AdRequestResult.FrequencyCapReached:
                default:
                    // The ad didn't load, either because it failed to download or the ad bid wasn't successful.
                    /* Write code here to:
                        - Disable any ad related UI you might be showing.
                        - Continue the game without giving the player a reward. */
                    this.Track(AdLoadFailedParams.Audio(adRequestResult.ToString(), adRequestResult.ToString()));
                    break;
            }
        }

        /// <summary>
        /// This callback is invoked when the ad begins to play.
        /// </summary>
        private void _OnAdPlaybackStarted(string placement, IAudioAd audioAd)
        {
            this.LogInfo(nameof(placement), placement, nameof(audioAd), audioAd.ToDict(), "impression", audioAd.ToAdImpression());
            /* Write code here to:
             - Turn down your game volume.
             - Turn off your game music.
             - Give your players an instant reward? */
            _audioService.RequestMute(_muteRequest);
            this.Track(audioAd.ToAdImpression());
            SharedCoreEvents.AudioAds.InvokeAdPlaybackStartedHandlers();
        }

        /// <summary>
        /// This callback is invoked when the ad finishes playing.
        /// </summary>
        private void _OnAdPlaybackCompleted(string placement, AdPlaybackStatus adPlaybackResult)
        {
            this.LogInfo(nameof(placement), placement, nameof(adPlaybackResult), adPlaybackResult.ToString());
            /* Write code here to:
             - Turn up your game volume. 
             - Turn on your game music. 
             - Disable any ad related UI you might be showing. */
            _audioService.RemoveMuteRequest(_muteRequest);
            switch (adPlaybackResult)
            {
                case AdPlaybackStatus.Finished:
                case AdPlaybackStatus.Skipped:
                    {
                        /* Write code here to: - Give your player a reward for listening to the ad? */
                        this.Track(AdClosedParams.Audio(placement));
                    }
                    break;
                    
                case AdPlaybackStatus.Stopped:
                case AdPlaybackStatus.Canceled: 
                    break;

                case AdPlaybackStatus.PlaybackFailed:
                default: 
                    this.Track(AdShowFailedParams.Audio(placement, adPlaybackResult.ToString(), adPlaybackResult.ToString()));
                    break;
            }
            
            SharedCoreEvents.AudioAds.InvokeAdPlaybackCompletedHandlers();
        }

        private void _OnSkipButtonAvailable(string placement)
        {
            this.LogInfo(nameof(placement), placement);
            /* Write code here to: - Display your own skip button if you have one. */
            this.Track(AdShowSucceededParams.Audio(placement));
        }
        
        private void _OnAdPlaybackStatusChangedEvent(string placement, AdSequence adSequence, AdPlaybackStatus adPlaybackStatus)
        {
            this.LogInfo(nameof(placement), placement, nameof(adPlaybackStatus), adPlaybackStatus.ToString());
    
            switch (adPlaybackStatus)
            {
                case AdPlaybackStatus.Started:
                    // replace _OnAdPlaybackStarted
                    _audioService.RequestMute(_muteRequest);
                    SharedCoreEvents.AudioAds.InvokeAdPlaybackStartedHandlers();
                    break;
                    
                case AdPlaybackStatus.Finished:
                case AdPlaybackStatus.Skipped:
                    _audioService.RemoveMuteRequest(_muteRequest);
                    this.Track(AdClosedParams.Audio(placement));
                    SharedCoreEvents.AudioAds.InvokeAdPlaybackCompletedHandlers();
                    break;
                    
                case AdPlaybackStatus.PlaybackFailed:
                case AdPlaybackStatus.RequestFailed:
                    _audioService.RemoveMuteRequest(_muteRequest);
                    this.Track(AdShowFailedParams.Audio(placement, adPlaybackStatus.ToString(), adPlaybackStatus.ToString()));
                    SharedCoreEvents.AudioAds.InvokeAdPlaybackCompletedHandlers();
                    break;
                    
                case AdPlaybackStatus.Stopped:
                case AdPlaybackStatus.Canceled:
                    _audioService.RemoveMuteRequest(_muteRequest);
                    SharedCoreEvents.AudioAds.InvokeAdPlaybackCompletedHandlers();
                    break;
                    
                case AdPlaybackStatus.Paused:
                case AdPlaybackStatus.Resumed:
                    break;
            }
        }

        private void _OnAdPaid(string placement, AdSequence adSequence, IAudioAd audioAd)
        {
            this.LogInfo(nameof(placement), placement, nameof(audioAd), audioAd.ToDict(), "impression", audioAd.ToAdImpression());
            this.Track(audioAd.ToAdImpression()); // Revenue tracking
        }

        private void _OnAdClicked(string placement, AdSequence adSequence)
        {
            this.LogInfo(nameof(placement), placement);
            this.Track(AdClickedParams.Audio(placement));
        }

        private void _OnAdFailed(string placement, AdSequence adSequence, string error)
        {
            this.LogInfo(nameof(placement), placement, nameof(error), error);
            this.Track(AdLoadFailedParams.Audio(error, error));
        }
    }
}
#endif