#if AUDIO_ADS
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Shared.Core.Async;
using Shared.Core.IoC;
using Shared.Core.Validator;
using Shared.Entity.AudioAds;
using Shared.Entity.Config;
using Shared.Repository.AudioAds;
using Shared.Repository.Level;
using Shared.Repository.Premium;
using Shared.Service.AudioAds.Internal;
using Shared.Service.AudioAds.Validator;
using Shared.Service.NotificationCenter;
using Shared.Service.SharedCoroutine;
using Shared.Utilities;
using Shared.Utils;
using Shared.View.AudioAds;
using UnityEngine;
using Zenject;
using IInitializable = Shared.Core.Handler.Corou.Initializable.IInitializable;

namespace Shared.Service.AudioAds
{
    [Service]
    public class AudioAdsService : IAudioAdsService, IInitializable, ISharedUtility, ISharedLogTag
    {
        private static readonly Dictionary<string, AudioAdSource> KeyAdSourceMap = new()
        {
            { "gadsme", AudioAdSource.Gadsme},
            { "audiomob", AudioAdSource.AudioMob },
            { "odeeo", AudioAdSource.Odeoo},
        };

        [Inject] private AudioAdsConfigRepository _configRepository;
        [Inject] private LevelRepository _levelRepository;
        [Inject] private PremiumEnableRepository _premiumEnableRepository;
        [Inject] private IConfig _config;
        
        private IValidator _validator;
        private IValidator Validator => _validator ??= ValidatorChain.CreateChainFromType<IAudioAdValidator>();
        
        private AudioAdPlacement _currentPlacement;
        private ISharedAudioAd _currentAudioAd;
        
        private bool Validate() => Validator?.Validate() ?? true;

        private IAsyncOperation _playOperation;
        private Coroutine _playCoroutine;

        private bool _isPause = false;

        public bool IsInitialized { get; private set; }
        private IAsyncOperation _initOperation;
        public IAsyncOperation Initialize()
        {
            if (_initOperation != null) return _initOperation;
            _initOperation = new SharedAsyncOperation().Success();
            IsInitialized = true;
            
            AudioAdRegistry.OnNewPlacementAddedEvent += placement =>
            {
                _currentAudioAd?.MoveTo(placement);
            };

            AudioAdRegistry.OnPlacementRemovedEvent += placement =>
            {
                this.StartSharedCoroutine(_OnPlacementRemovedEvent(placement));
            };

            return _initOperation;
        }

        public IAsyncOperation TryPlay()
        {
            if (_playOperation != null) return _playOperation;

            var validateResult = Validate();
            if (!validateResult)
            {
                this.LogWarning(nameof(validateResult), false);
                return new SharedAsyncOperation().Fail("!validationResult");
            }

            if (AudioAdRegistry.PlacementStack.Count <= 0)
            {
                this.LogWarning(nameof(AudioAdRegistry.PlacementStack.Count), AudioAdRegistry.PlacementStack.Count);
                return new SharedAsyncOperation().Fail("AudioAdRegistry.PlacementStack.Count == 0");
            }

            var topPlacement = AudioAdRegistry.PlacementStack[0];
            var config = _configRepository.Get();
            var ads = _GetPriorityAds(config);

            _playOperation = new SharedAsyncOperation();
            _playCoroutine = this.StartSharedCoroutine(_Play(ads, topPlacement));

            return _playOperation;
        }

        private IEnumerator _Play(List<ISharedAudioAd> ads, AudioAdPlacement placement)
        {
            _currentPlacement = placement;
            var isSuccess = false;
            foreach (var a in ads)
            {
                while (_isPause) yield return null;
                _currentAudioAd = a;
                this.LogInfo("f", nameof(_Play), nameof(a), a.GetType().Name);
                var o = a.Play(_currentPlacement);
                if (o == null) continue;
                if (!o.IsComplete) SharedNotificationCenter.Notify(NotificationId.AudioAdStarted);
                while (!o.IsComplete) yield return null;
                _currentAudioAd = null;
                if (!o.IsSuccess) continue;
                isSuccess = true;
                SharedNotificationCenter.Notify(NotificationId.AudioAdCompleted);
                break;
            }

            if (isSuccess) _playOperation.Success();
            else _playOperation.Fail("Audio Ad Placement Failed");
            _playOperation = null;
        }

        private List<ISharedAudioAd> _GetPriorityAds(AudioAdsConfig config)
        {
            var scoringDict = _ToScoringDict(config.Priorities);
            var cleanAds = AudioAdRegistry.AudioAds.Where(ad => scoringDict.ContainsKey(ad.AudioAdSource)).ToList();
            return cleanAds.OrderBy(ad => scoringDict.GetValueOrDefault(ad.AudioAdSource, int.MaxValue)).ToList();
        }

        private static Dictionary<AudioAdSource, int> _ToScoringDict(List<AudioAdsConfig.Priority> priorities)
        {
            Dictionary<AudioAdSource, int> scoreDict = new();
            for (var i = 0; i < priorities.Count; i++)
            {
                scoreDict.Add(KeyAdSourceMap[priorities[i].AdType], i); 
            }
            return scoreDict;
        }


        public void Stop()
        {
            this.LogInfo();
            if (_playCoroutine != null)
            {
                this.StopSharedCoroutine(_playCoroutine);
                _playCoroutine = null;
            }

            if (_currentAudioAd != null)
            {
                _currentAudioAd.Stop();
                _currentAudioAd = null;
            }
            
            _playOperation?.Fail("Stop");
            _playOperation = null;
            
            _currentPlacement = null;
        }

        public void Pause()
        {
            _isPause = true;
            _currentAudioAd?.Pause();
            
        }

        public void Resume()
        {
            _isPause = false;
            _currentAudioAd?.Resume();
        }

        public string LogTag => SharedLogTag.AudioAds;

        private IEnumerator _OnPlacementRemovedEvent(AudioAdPlacement placement)
        {
            yield return null;
            
            if (_currentAudioAd == null) yield break;
            if (AudioAdRegistry.PlacementStack.Count <= 0)
            {
                Stop();
                yield break;
            }
                
            var topPlacement = AudioAdRegistry.PlacementStack[0];
            _currentAudioAd?.MoveTo(topPlacement);
        }
    }
}
#endif