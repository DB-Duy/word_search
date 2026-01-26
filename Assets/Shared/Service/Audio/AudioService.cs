using System;
using System.Collections.Generic;
using System.Linq;
using ModestTree;
using Service.Audio.Internal;
using Shared.Core.Async;
using Shared.Core.IoC;
using Shared.Core.Repository.Audio;
using Shared.Repository.Audio;
using Shared.Service.Audio.Internal;
using Shared.Utilities;
using Shared.Utils;
using UnityEngine;
using Zenject;
using IInitializable = Shared.Core.Handler.Corou.Initializable.IInitializable;

namespace Shared.Service.Audio
{
    [Service]
    public class AudioService : IAudioService, ISharedUtility, IInitializable, ISharedLogTag
    {
        [Inject] private CacheAudioClipRepository _clipRepository;
        [Inject] private AudioEnableRepository _enableRepository;
        [Inject] private TemporaryAudioEnableRepository _temporaryEnableRepository;
        
        private AudioSource _musicAudioSource;
        private readonly List<AudioSource> _cacheSources = new();
        private readonly Dictionary<string, IMuteRequest> _muteRequests = new();
        private string _lastMusicRequest = string.Empty;
        
        public bool IsInitialized { get; private set; }
        private IAsyncOperation _initOperation;
        public IAsyncOperation Initialize()
        {
            if (_initOperation != null) return _initOperation;
            
            _enableRepository.onValueChanged.AddListener(_OnTemporaryEnableValueChanged);
            _temporaryEnableRepository.onValueChanged.AddListener(_OnTemporaryEnableValueChanged);
            _temporaryEnableRepository.Set(_ValidateAudioEnable());
            _GetOrCreateMusicSource();
            
            IsInitialized = true;
            _initOperation = new SharedAsyncOperation().Success();
            return _initOperation;
        }
        
        private void _OnTemporaryEnableValueChanged(bool isEnable)
        {
            if (!isEnable)
            {
                StopSounds();
                PauseMusic();    
            }
            else
            {
                ResumeMusic();
            }
        }
        
        /// ------------------------------------------------------------------------------------------------------------
        /// APIs
        /// ------------------------------------------------------------------------------------------------------------
        public AudioSource PlaySound(string name, float time = 1, float pitch = 1, float volume = 1)
        {
            var isAudioEnable = _ValidateAudioEnable();
            if (!isAudioEnable) return null;

            var c = _clipRepository.Get(name);
            if (c == null) throw new Exception($"{nameof(AudioService)}->PlaySound: c == null for {name}");
            var audioSource = _GetOrCreateSoundSource();
            if (audioSource == null) return null;
            audioSource.volume = volume;
            audioSource.pitch = pitch;
            audioSource.clip = c;
            audioSource.Play();

            return audioSource;
        }

        public AudioSource PlayMusic(string name, float pitch = 1, float volume = 1)
        {
            var isAudioEnable = _ValidateAudioEnable();
            _lastMusicRequest = name;
            if (!isAudioEnable)
            {
                return null;
            }
            var c = _clipRepository.Get(name);
            if (c == null) throw new Exception($"{nameof(AudioService)}->PlaySound: c == null for {name}");
            var audioSource = _GetOrCreateMusicSource();
            audioSource.volume = volume; //* (SaveData.I.soundSfx ? 1f : 0f);
            audioSource.pitch = pitch;
            audioSource.loop = true;
            audioSource.clip = c;
            audioSource.Play();
            return audioSource;
        }

        public AudioSource PlayButton() => PlaySound("button");

        public void StopSounds()
        {
            foreach (var s in _cacheSources) s.Stop();
        }

        public void PauseMusic()
        {
            this.LogInfo(nameof(_musicAudioSource.isPlaying), _musicAudioSource?.isPlaying);
            if (_musicAudioSource == null || !_musicAudioSource.isPlaying) return;
            _musicAudioSource.Pause();
        }

        public void ResumeMusic()
        {
            this.LogInfo(nameof(_musicAudioSource.isPlaying), _musicAudioSource?.isPlaying);
            if (_musicAudioSource == null || _musicAudioSource.isPlaying) return;
            if (_clipRepository.Get(_lastMusicRequest) == _musicAudioSource.clip)
            {
                this.LogInfo("call", nameof(_musicAudioSource.UnPause));
                _musicAudioSource.UnPause();
            }
            else
            {
                PlayMusic(_lastMusicRequest);
            }
        }

        /// ------------------------------------------------------------------------------------------------------------
        /// MUTE Sector
        /// ------------------------------------------------------------------------------------------------------------
        public void RequestMute(MuteRequest r)
        {
            this.LogInfo(nameof(r), r);
            if (_muteRequests.ContainsKey(r.Name)) return;
            _muteRequests.Add(r.Name, r);
            var isAudioEnable = _ValidateAudioEnable();
            _temporaryEnableRepository.Set(isAudioEnable);
        }

        public void RemoveMuteRequest(MuteRequest r)
        {
            this.LogInfo(nameof(r), r);
            if (!_muteRequests.ContainsKey(r.Name)) return;
            _muteRequests.Remove(r.Name);
            var isAudioEnable = _ValidateAudioEnable();
            _temporaryEnableRepository.Set(isAudioEnable);
        }

        public void RequestMute(string name)
        {
            if (_muteRequests.ContainsKey(name)) return;
            RequestMute(new MuteRequest(name));
        }

        public void RemoveMuteRequest(string name)
        {
            if (!_muteRequests.ContainsKey(name)) return;
            RemoveMuteRequest((MuteRequest)_muteRequests[name]);
        }

        private bool _ValidateAudioEnable() => _enableRepository.Get() && _muteRequests.IsEmpty();
        // --------------------------------------------------------------------------------------
        private AudioSource _GetOrCreateSoundSource()
        {
            foreach (var s in _cacheSources.Where(s => !s.isPlaying && !s.loop))
            {
                // Move playing one to the end of the queue.
                _cacheSources.Remove(s);
                _cacheSources.Add(s);
                return s;
            }

            var newOne = this.GetSharedGameObject().AddComponent<AudioSource>();
            _cacheSources.Add(newOne);
            return newOne;
        }

        private AudioSource _GetOrCreateMusicSource()
        {
            if (_musicAudioSource == null)
                _musicAudioSource = this.GetSharedGameObject().AddComponent<AudioSource>();
            return _musicAudioSource;
        }

        public string LogTag => SharedLogTag.Audio;
    }
}