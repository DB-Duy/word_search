// using System;
// using System.Collections.Generic;
// using Shared.Common;
// using Shared.Core.IoC;
// using Shared.Core.Repository.IntType;
// using Shared.Core.Repository.RemoteConfig;
// using Shared.Repository.RemoteConfig.Models;
// using Shared.Service.Session.Internal;
// using Shared.Tracking;
// using Shared.Tracking.Models.Session;
// using Shared.Tracking.Property;
// using Shared.Utils;
// using UnityEngine;
//
// namespace Shared.SharedSession
// {
//     public class IosSessionController : IoCMonoBehavior, ISessionController, ISharedUtility
//     {
//         private const string TAG = "IosSessionController";
//         
//         private DateTime _pausedDateTime;
//         private long _sessionId;
//         private string _currentActivityName;
//         
//         string ISessionController.SessionId => _sessionId.ToString();
//
//         public bool IsInitialized { get; private set; }
//         private IIntRepository _countRepository;
//         private IRemoteConfigRepository<SessionTimeoutConfig> _configRepository;
//         private readonly HashSet<INewSessionHandler> _newSessionHandlers = new();
//         private IAsyncOperation _initOperation;
//
//         public IAsyncOperation Initialize()
//         {
//             if (_initOperation != null) return _initOperation;
//             IsInitialized = true;
//             _initOperation = new SharedAsyncOperation().Success();
//             _NewSession();
//             return _initOperation;
//         }
//         
//         private void OnApplicationPause(bool pause)
//         {
//             if (!IsInitialized) return;
//             SharedLogger.Log($"{TAG}->OnApplicationPause: {pause}");
//             if (pause)
//             {
//                 _pausedDateTime = DateTime.Now;
//                 _PauseSession();
//             }
//             else
//             {
//                 _ResumeOrNewSession();
//             }
//         }
//
//
//         public ISessionController SetUp(IIntRepository countRepository, IRemoteConfigRepository<SessionTimeoutConfig> remoteConfigRepository)
//         {
//             _countRepository = countRepository;
//             _configRepository = remoteConfigRepository;
//             return this;
//         }
//
//         public void AddNewSessionHandlers(params INewSessionHandler[] handlers)
//         {
//             _newSessionHandlers.AddRange(handlers);
//         }
//
//         private void _PauseSession()
//         {
//             SharedLogger.Log($"{TAG}->_PauseSession {_sessionId}");
//             this.Track(new SessionPauseEvent(_sessionId, "null", "null"));
//         }
//
//         private void _ResumeOrNewSession()
//         {
//             if(!IsInitialized) return;
//             SharedLogger.Log($"{TAG}->_ResumeOrNewSession {_sessionId}");
//             var config = _configRepository.Get();
//             var interval = (DateTime.Now - _pausedDateTime).TotalSeconds;
//             if (interval >= config.SessionTimeout)
//             {
//                 SharedLogger.Log($"{TAG}->_ResumeOrNewSession _NewSession(activityName); by {config}");
//                 _NewSession();
// #if FIREBASE || FIREBASE_WEBGL
//                 SharedLogger.Log($"{TAG}->_ResumeOrNewSession->_NewSession->SharedCore.Instance.FirebaseRemoteConfigService.Fetch();");
//                 SharedCore.Instance.FirebaseRemoteConfigService.Fetch();
// #endif
//             }
//             else _ResumeSession();
//         }
//
//         private void _ResumeSession()
//         {
//             SharedLogger.Log($"{TAG}->_ResumeSession: {_sessionId}");
//             SharedCore.Instance.TrackingService.Track(new SessionResumeEvent(_sessionId, "null", "null"));
//         }
//
//         private void _NewSession()
//         {
//             _sessionId = SessionUtils.NewSessionId();
//             var count = _countRepository.AddMore(1);
//             SharedLogger.Log($"{TAG}->_NewSession: sessionId={_sessionId}, count={count}");
//             SharedCore.Instance.TrackingService.Track(new SessionStartEvent(_sessionId, "null", "null"));
//             foreach (var handler in _newSessionHandlers) handler.Handle(count);
//             SharedCore.Instance.TrackingService.SetProperty(PropertyConst.SESSION_ID, _sessionId);
//             SharedCoreEvents.Session.InvokeNewSessionHandlers(_sessionId);
//             // var cachedData = SharedCore.Instance.TrackingService.GetGameInterruptedData();
//             // cachedData[PropertyConst.SESSION_ID] = _sessionId;
//             // SharedCore.Instance.TrackingService.PrepareGameInterruptedData(cachedData);
//         }
//     }
// }