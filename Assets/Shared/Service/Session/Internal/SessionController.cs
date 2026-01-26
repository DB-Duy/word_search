// using System;
// using System.Collections.Generic;
// using Shared.Common;
// using Shared.Core.Repository.IntType;
// using Shared.Core.Repository.RemoteConfig;
// using Shared.Repository.RemoteConfig.Models;
// using Shared.Service.Session.Internal;
// using Shared.Utils;
// using UnityEngine;
// using UnityEngine.Events;
//
// namespace Shared.SharedSession
// {
//     [DisallowMultipleComponent]
//     public class SessionController : MonoBehaviour, ISessionController
//     {
//         public class OnFirstSessionEvent : UnityEvent<long>
//         {
//         }
//
//         public class OnNewSessionEvent : UnityEvent<long>
//         {
//         }
//
//         private long _sessionId;
//         string ISessionController.SessionId => _sessionId.ToString();
//
//         // ReSharper disable once InconsistentNaming
//         private const float SESSION_REQUIRED_MINUTES = 30;
//
//         // ReSharper disable once ArrangeTypeMemberModifiers
//         const string Tag = "SessionController";
//         private readonly HashSet<INewSessionHandler> _newSessionHandlers = new();
//
//         private IIntRepository _counterRepository;
//         private DateTime _lastActiveDatetime;
//
//         public bool IsInitialized { get; private set; } = false;
//
//         public ISessionController SetUp(IIntRepository countRepository,
//             IRemoteConfigRepository<SessionTimeoutConfig> remoteConfigRepository)
//         {
//             _counterRepository = countRepository;
//             return this;
//         }
//
//         public IAsyncOperation Initialize()
//         {
//             if (IsInitialized) return new SharedAsyncOperation().Success();
//             IsInitialized = true;
//             _lastActiveDatetime = DateTime.Now;
//             var count = _counterRepository.AddMore(1);
//             SharedLogger.Log($"{Tag}->NewSession: {count}");
//             SharedCoreEvents.Session.InvokeNewSessionHandlers(_sessionId);
//             foreach (var handler in _newSessionHandlers) handler.Handle(count);
//             return new SharedAsyncOperation().Success();
//         }
//
//         public void AddNewSessionHandlers(params INewSessionHandler[] handlers) =>
//             _newSessionHandlers.AddRange(handlers);
//
//         public string SessionId { get; }
//
//         private void OnApplicationPause(bool pause)
//         {
//             SharedLogger.Log($"{Tag}->OnApplicationPause: {pause}");
//             if (pause) _lastActiveDatetime = DateTime.Now;
//             else _CalculateSession();
//         }
//
//         private void _CalculateSession()
//         {
//             var timeSpan = DateTime.Now - _lastActiveDatetime;
//             if (timeSpan.TotalMinutes >= SESSION_REQUIRED_MINUTES)
//             {
//                 _lastActiveDatetime = DateTime.Now;
//                 _sessionId = SessionUtils.NewSessionId();
//                 var count = _counterRepository.AddMore(1);
//                 SharedLogger.Log($"{Tag}->NewSession: {count}");
//                 SharedCoreEvents.Session.InvokeNewSessionHandlers(_sessionId);
//                 foreach (var handler in _newSessionHandlers) handler.Handle(count);
//             }
//         }
//     }
// }