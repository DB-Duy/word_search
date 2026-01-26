#if UNITY_IOS || UNITY_EDITOR
using System;
using Shared.Core.Async;
using Shared.Core.Handler;
using Shared.Core.IoC;
using Shared.Core.IoC.UnityLifeCycle;
using Shared.Entity.Session;
using Shared.Repository.Session;
using Shared.Service.Session.Handler;
using Shared.Service.Session.Internal;
using Shared.Utilities;
using Shared.Utils;
using Zenject;

namespace Shared.Service.Session
{
    [Service]
    public class DefaultSessionService : ISessionService, ISharedUtility, ISharedLogTag, IUnityOnApplicationPause
    {
#if GOOGLE_PLAY || APPSTORE || APPSTORE_CHINA
        [Inject] private SessionConfigRepository _configRepository;
#endif
        [Inject] private SessionCountRepository _countRepository;
        [Inject] private SessionRepository _sessionRepository;

        private IHandler<SessionEntity> _onNewSessionHandler;

        private IHandler<SessionEntity> OnNewSessionHandler => _onNewSessionHandler ??=
            SequenceHandlerChain<SessionEntity>.CreateChainFromType<ISessionCreatedHandler>();

        private IHandler<SessionEntity, NativeClassEntity> _onSessionPausedHandler;

        private IHandler<SessionEntity, NativeClassEntity> OnSessionPausedHandler => _onSessionPausedHandler ??=
            SequenceHandlerChain<SessionEntity, NativeClassEntity>.CreateChainFromType<ISessionPausedHandler>();

        private IHandler<SessionEntity, NativeClassEntity> _onSessionResumedHandler;

        private IHandler<SessionEntity, NativeClassEntity> OnSessionResumedHandler => _onSessionResumedHandler ??=
            SequenceHandlerChain<SessionEntity, NativeClassEntity>.CreateChainFromType<ISessionResumedHandler>();

        public long SessionId { get; private set; }

        private DateTime _pausedDateTime;

        public bool IsInitialized { get; private set; } = false;

        public IAsyncOperation Initialize()
        {
            if (IsInitialized) return new SharedAsyncOperation().Success();
            IsInitialized = true;
            this.LogInfo(nameof(IsInitialized), IsInitialized);
            _NewSession(isFromResume: false);
            return new SharedAsyncOperation().Success();
        }

        public void OnApplicationPause(bool pause)
        {
            this.LogInfo(nameof(pause), pause);
            if (pause)
            {
                _pausedDateTime = DateTime.Now;
                _PauseSession();
            }
            else
            {
                _ResumeOrNewSession();
            }
        }

        private void _ResumeOrNewSession()
        {
#if GOOGLE_PLAY || APPSTORE || APPSTORE_CHINA
            if (!IsInitialized) return;
            this.LogInfo(nameof(_pausedDateTime), _pausedDateTime);
            var config = _configRepository.Get();
            var interval = (DateTime.Now - _pausedDateTime).TotalSeconds;
            if (interval >= config.SessionTimeout)
            {
                _NewSession(isFromResume: true);
            }
            else _ResumeSession();
#else
            _NewSession(isFromResume: true);
#endif
        }

        private void _NewSession(bool isFromResume)
        {
            SessionId = SessionUtils.NewSessionId();
            var count = _countRepository.AddMore(1);

            var e = SessionEntity.NewInstance(SessionId, count, isFromResume);
            _sessionRepository.Save(e);
            this.LogInfo(nameof(e), e, nameof(count), count, nameof(isFromResume), isFromResume);

            OnNewSessionHandler?.Handle(e);
            SharedCoreEvents.Session.InvokeNewSessionHandlers(SessionId);
        }

        private void _PauseSession()
        {
            this.LogInfo(nameof(SessionId), SessionId);
            var e = _sessionRepository.Get();
            OnSessionPausedHandler?.Handle(e, null);
        }

        private void _ResumeSession()
        {
            this.LogInfo(nameof(SessionId), SessionId);
            var sessionEntity = _sessionRepository.Get();
            OnSessionResumedHandler.Handle(sessionEntity, null);
        }

        public string LogTag => SharedLogTag.Session;
    }
}
#endif