using System.Collections.Generic;
using Shared.Core.Async;
using Shared.Core.IoC;
using Shared.Service.AudioAds;
using Shared.Service.NotificationCenter;
using Zenject;
using IInitializable = Shared.Core.Handler.Corou.Initializable.IInitializable;

namespace Shared.Service.Audio
{
    [Service]
    public class AudioMediator : IInitializable, ISharedUtility
    {
        private const string Interstitial = "InterstitialAd";
        private const string RewardedVideo = "RewardedVideo";
        private const string AudioAd = "AudioAd";

        [Inject] private IAudioService _audioService;
        [Inject(Optional = true)] private IAudioAdsService _audioAdsService;
        
        private IAsyncOperation _initOperation;
        public bool IsInitialized { get; private set; }
        public IAsyncOperation Initialize()
        {
            if (_initOperation != null) return _initOperation;
            
            SharedNotificationCenter.Register(NotificationId.InterstitialStarted, _Mediate);
            SharedNotificationCenter.Register(NotificationId.InterstitialCompleted, _Mediate);
            
            SharedNotificationCenter.Register(NotificationId.RewardedStarted, _Mediate);
            SharedNotificationCenter.Register(NotificationId.RewardedCompleted, _Mediate);
            
            SharedNotificationCenter.Register(NotificationId.AudioAdStarted, _Mediate);
            SharedNotificationCenter.Register(NotificationId.AudioAdCompleted, _Mediate);
            
            IsInitialized = true;
            return _initOperation = new SharedAsyncOperation().Success();
        }

        private void _Mediate(string cmd, Dictionary<string, object> data)
        {
            switch (cmd)
            {
                case NotificationId.InterstitialStarted:
                {
                    _audioService?.RequestMute(Interstitial);
                    _audioAdsService?.Pause();
                    break;
                }
                case NotificationId.InterstitialCompleted:
                {
                    _audioService?.RemoveMuteRequest(Interstitial);
                    _audioAdsService?.Resume();
                    break;
                }
                case NotificationId.RewardedStarted:
                {
                    _audioService?.RequestMute(RewardedVideo);
                    _audioAdsService?.Pause();
                    break;
                }
                case NotificationId.RewardedCompleted:
                {
                    _audioService?.RemoveMuteRequest(RewardedVideo);
                    _audioAdsService?.Resume();
                    break;
                }
                case NotificationId.AudioAdStarted:
                {
                    _audioService?.RequestMute(AudioAd);
                    break;
                }
                case NotificationId.AudioAdCompleted:
                {
                    _audioService?.RemoveMuteRequest(AudioAd);
                    break;
                }
            }
        }
    }
}