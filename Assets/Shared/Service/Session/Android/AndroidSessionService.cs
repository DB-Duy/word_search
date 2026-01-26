#if GOOGLE_PLAY && !UNITY_EDITOR
using System;
using Shared.Core.Async;
using Shared.Core.Handler;
using Shared.Core.IoC;
using Shared.Entity.Session;
using Shared.Repository.Session;
using Shared.Service.Session.Handler;
using Shared.Service.Session.Internal;
using Shared.Utilities;
using Shared.Utils;
using UnityEngine;
using Zenject;

namespace Shared.Service.Session.Android
{
    [Service]
    public class AndroidSessionService : ISessionService, IIoC, ISharedUtility, ISharedLogTag
    {
        private const string UnityActivityName = "com.unity3d.player.UnityPlayerActivityWithANRWatchDog";
        
        [Inject] private SessionConfigRepository _configRepository;
        [Inject] private SessionCountRepository _countRepository;
        [Inject] private SessionRepository _sessionRepository;
        
        private IHandler<SessionEntity> _onNewSessionHandler;
        private IHandler<SessionEntity> OnNewSessionHandler => _onNewSessionHandler ??= SequenceHandlerChain<SessionEntity>.CreateChainFromType<ISessionCreatedHandler>();

        private IHandler<SessionEntity, NativeClassEntity> _onSessionPausedHandler;
        private IHandler<SessionEntity, NativeClassEntity> OnSessionPausedHandler => _onSessionPausedHandler ??= SequenceHandlerChain<SessionEntity, NativeClassEntity>.CreateChainFromType<ISessionPausedHandler>();

        private IHandler<SessionEntity, NativeClassEntity> _onSessionResumedHandler;
        private IHandler<SessionEntity, NativeClassEntity> OnSessionResumedHandler => _onSessionResumedHandler ??= SequenceHandlerChain<SessionEntity, NativeClassEntity>.CreateChainFromType<ISessionResumedHandler>();

        public long SessionId { get; private set; }

        private IAndroidApplicationLifeCycleListener _androidApplicationLifeCycleListener;
        private DateTime _pausedDateTime;
        private string _currentActivityName;
        
        private IAsyncOperation _initOperation;
        public bool IsInitialized { get; private set; }

        public IAsyncOperation Initialize()
        {
            if (_initOperation != null) return _initOperation;
            
            this.LogInfo();
            _androidApplicationLifeCycleListener = new AndroidApplicationLifeCycleListener();
            AndroidApplicationEvents.OnActivityResumedEvent += _OnActivityResumedEvent;
            AndroidApplicationEvents.OnActivityPausedEvent += _OnActivityPausedEvent;
            Application.quitting += _OnUnityApplicationQuittingEvent;
            Application.wantsToQuit += _OnUnityApplicationWantsToQuit;
            _currentActivityName = UnityActivityName;

            IsInitialized = true;
            _initOperation = new SharedAsyncOperation().Success();
            _NewSession(UnityActivityName, isFromResume: false);
            return _initOperation;
        }

        // -------------------------------------------------------------------------------------------------------------
        // Activity Life Cycle
        // -------------------------------------------------------------------------------------------------------------
        private void _OnActivityResumedEvent(string activityName)
        {
            this.LogInfo(nameof(activityName), activityName);
            _currentActivityName = activityName;
            _ResumeOrNewSession(activityName);
        }
        
        private void _OnActivityPausedEvent(string activityName)
        {
            this.LogInfo(nameof(activityName), activityName);
            _pausedDateTime = DateTime.Now;
            _PauseSession(activityName);
        }

        private void _OnUnityApplicationQuittingEvent()
        {
            this.LogInfo(nameof(_currentActivityName), _currentActivityName);
        }
        
        /// <summary>
        /// https://docs.unity3d.com/ScriptReference/Application-wantsToQuit.html
        /// </summary>
        /// <returns></returns>
        private bool _OnUnityApplicationWantsToQuit()
        {
            try
            {
                _PauseSession(_currentActivityName);
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }
            return true;
        }
        // -------------------------------------------------------------------------------------------------------------
        // Handle
        // -------------------------------------------------------------------------------------------------------------
        private void _PauseSession(string activityName)
        {
            this.LogInfo(nameof(SessionId), SessionId, nameof(activityName), activityName);
            var e = _sessionRepository.Get();
            var nativeClass = new NativeClassEntity(activityName);
            OnSessionPausedHandler?.Handle(e, nativeClass);
        }

        private void _ResumeOrNewSession(string activityName)
        {
            this.LogInfo(nameof(activityName), activityName);
            var config = _configRepository.Get();
            var interval = (DateTime.Now - _pausedDateTime).TotalSeconds;
            if (interval >= config.SessionTimeout)
            {
                _NewSession(activityName, isFromResume: true);
            }
            else _ResumeSession(activityName);
        }

        private void _ResumeSession(string activityName)
        {
            this.LogInfo(nameof(SessionId), SessionId, nameof(activityName), activityName);
            var sessionEntity = _sessionRepository.Get();
            var nativeClass = new NativeClassEntity(activityName);
            OnSessionResumedHandler.Handle(sessionEntity, nativeClass);
        }

        private void _NewSession(string activityName, bool isFromResume)
        {
            SessionId = SessionUtils.NewSessionId();
            var count = _countRepository.AddMore(1);
            
            var e = SessionEntity.NewInstance(SessionId, count, isFromResume, activityName);
            _sessionRepository.Save(e);
            this.LogInfo(nameof(e), e, nameof(count), count, nameof(isFromResume), isFromResume, nameof(activityName), activityName);
            
            OnNewSessionHandler?.Handle(e);
            SharedCoreEvents.Session.InvokeNewSessionHandlers(SessionId);
        }

        public string LogTag => SharedLogTag.Session;
    }
}
#endif