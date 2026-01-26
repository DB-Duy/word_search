// using System;
// using System.Collections.Generic;
// using Shared.App;
// using Shared.Common;
// using Shared.Core.Repository.IntType;
// using Shared.Core.Repository.RemoteConfig;
// using Shared.Repository.RemoteConfig;
// using Shared.Repository.RemoteConfig.Models;
// using Shared.Service.Session.Internal;
// using Shared.Service.Tracking;
// using Shared.Tracking;
// using Shared.Tracking.Models.Session;
// using Shared.Tracking.Property;
// using Shared.Utils;
// using UnityEngine;
//
// namespace Shared.SharedSession
// {
//     /// <summary>
//     /// 1. Case show ads
//     /// onActivityPaused: com.unity3d.player.UnityPlayerActivityWithANRWatchDog
//     /// onActivityCreated: com.ironsource.mediationsdk.testSuite.TestSuiteActivity
//     /// onActivityStarted: com.ironsource.mediationsdk.testSuite.TestSuiteActivity
//     /// onActivityResumed: com.ironsource.mediationsdk.testSuite.TestSuiteActivity
//     /// onActivityStopped: com.unity3d.player.UnityPlayerActivityWithANRWatchDog
//     /// onActivitySaveInstanceState: com.unity3d.player.UnityPlayerActivityWithANRWatchDog
//     ///
//     /// 2. Case tắt ads
//     /// onActivityPaused: com.ironsource.mediationsdk.testSuite.TestSuiteActivity
//     /// onActivityStarted: com.unity3d.player.UnityPlayerActivityWithANRWatchDog
//     /// onActivityResumed: com.unity3d.player.UnityPlayerActivityWithANRWatchDog
//     /// onActivityStopped: com.ironsource.mediationsdk.testSuite.TestSuiteActivity
//     /// onActivityDestroyed: com.ironsource.mediationsdk.testSuite.TestSuiteActivity
//     ///
//     /// 3. Case đang show ads, user nhấn show all running apps.
//     /// onActivityCreated: com.ironsource.mediationsdk.testSuite.TestSuiteActivity
//     /// onActivityStarted: com.ironsource.mediationsdk.testSuite.TestSuiteActivity
//     /// onActivityResumed: com.ironsource.mediationsdk.testSuite.TestSuiteActivity
//     /// onActivityStopped: com.unity3d.player.UnityPlayerActivityWithANRWatchDog
//     /// onActivitySaveInstanceState: com.unity3d.player.UnityPlayerActivityWithANRWatchDog
//     /// onActivityPaused: com.ironsource.mediationsdk.testSuite.TestSuiteActivity
//     /// onActivityStopped: com.ironsource.mediationsdk.testSuite.TestSuiteActivity
//     /// onActivitySaveInstanceState: com.ironsource.mediationsdk.testSuite.TestSuiteActivity
//     ///
//     /// 4. Case đang show ads, user nhấn Home.
//     /// onActivityStarted: com.ironsource.mediationsdk.testSuite.TestSuiteActivity
//     /// onActivityResumed: com.ironsource.mediationsdk.testSuite.TestSuiteActivity
//     /// onActivityPaused: com.ironsource.mediationsdk.testSuite.TestSuiteActivity
//     /// onActivityStopped: com.ironsource.mediationsdk.testSuite.TestSuiteActivity
//     /// onActivitySaveInstanceState: com.ironsource.mediationsdk.testSuite.TestSuiteActivity
//     /// </summary>
//     public class AndroidSessionController : ISessionController
//     {
//         private const string Tag = "AndroidSessionController";
//         private const string UnityActivityName = "com.unity3d.player.UnityPlayerActivityWithANRWatchDog";
//         
//         private IIntRepository _countRepository;
//         private IRemoteConfigRepository<SessionTimeoutConfig> _configRepository;
//         private readonly HashSet<INewSessionHandler> _newSessionHandlers = new();
//         
//         private DateTime _pausedDateTime;
//         private long _sessionId;
//         private string _currentActivityName;
//         
//         public string SessionId => _sessionId.ToString();
//
//         public bool IsInitialized { get; private set; }
//         private readonly ITrackingService _tracking;
//         
//         private IAsyncOperation _initOperation;
//
//         public AndroidSessionController(ITrackingService tracking = null, IIntRepository countRepository = null, IRemoteConfigRepository<SessionTimeoutConfig> configRepository = null)
//         {
//             _tracking = tracking;
//             _countRepository = countRepository;
//             _configRepository = configRepository;
//         }
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
//         public IAsyncOperation Initialize()
//         {
//             if (_initOperation != null) return _initOperation;
//             if (SharedApplication.ApplicationLifeCycleListener is IAndroidApplicationLifeCycleListener applicationLifeCycleListener)
//             {
//                 applicationLifeCycleListener.OnActivityResumedEvent += _OnActivityResumedEvent;
//                 applicationLifeCycleListener.OnActivityPausedEvent += _OnActivityPausedEvent;
//             }
//
//             IsInitialized = true;
//             _initOperation = new SharedAsyncOperation().Success();
//             Application.quitting += _OnUnityApplicationQuittingEvent;
//             Application.wantsToQuit += _OnUnityApplicationWantsToQuit;
//             _currentActivityName = UnityActivityName;
//             _NewSession(UnityActivityName);
//             return _initOperation;
//         }
//
//         // -------------------------------------------------------------------------------------------------------------
//         // Activity Life Cycle
//         // -------------------------------------------------------------------------------------------------------------
//         private void _OnActivityResumedEvent(string activityName)
//         {
//             SharedLogger.Log($"{Tag}->_OnActivityResumedEvent: {activityName}");
//             _currentActivityName = activityName;
//             _ResumeOrNewSession(activityName);
//         }
//         
//         private void _OnActivityPausedEvent(string activityName)
//         {
//             SharedLogger.Log($"{Tag}->_OnActivityPausedEvent: {activityName}");
//             _pausedDateTime = DateTime.Now;
//             _PauseSession(activityName);
//         }
//
//         private void _OnUnityApplicationQuittingEvent()
//         {
//             SharedLogger.Log($"{Tag}->_OnUnityApplicationQuittingEvent: {_currentActivityName}");
//             // _PauseSession(_currentActivityName);
//         }
//         
//         /// <summary>
//         /// https://docs.unity3d.com/ScriptReference/Application-wantsToQuit.html
//         /// </summary>
//         /// <returns></returns>
//         private bool _OnUnityApplicationWantsToQuit()
//         {
//             try
//             {
//                 _PauseSession(_currentActivityName);
//             }
//             catch (Exception e)
//             {
//                 Debug.LogError(e);
//             }
//             return true;
//         }
//         // -------------------------------------------------------------------------------------------------------------
//         // Handle
//         // -------------------------------------------------------------------------------------------------------------
//         private void _PauseSession(string activityName)
//         {
//             SharedLogger.Log($"{Tag}->_PauseSession {_sessionId} for {activityName}");
//             var d = new JavaClassImpl(activityName);
//             SharedCore.Instance.TrackingService.Track(new SessionPauseEvent(_sessionId, className: d.ClassName, packageName: d.PackageName));
//         }
//
//         private void _ResumeOrNewSession(string activityName)
//         {
//             SharedLogger.Log($"{Tag}->_ResumeOrNewSession {_sessionId}");
//             var config = _configRepository.Get();
//             var interval = (DateTime.Now - _pausedDateTime).TotalSeconds;
//             if (interval >= config.SessionTimeout)
//             {
//                 SharedLogger.Log($"{Tag}->_ResumeOrNewSession _NewSession(activityName); by {config}");
//                 _NewSession(activityName);
// #if FIREBASE || FIREBASE_WEBGL
//                 SharedLogger.Log($"{Tag}->_ResumeOrNewSession->_NewSession->SharedCore.Instance.FirebaseRemoteConfigService.Fetch();");
//                 SharedCore.Instance.FirebaseRemoteConfigService.Fetch();
// #endif
//             }
//             else _ResumeSession(activityName);
//         }
//
//         private void _ResumeSession(string activityName)
//         {
//             SharedLogger.Log($"{Tag}->_ResumeSession: {_sessionId}");
//             var d = new JavaClassImpl(activityName);
//             _tracking.Track(new SessionResumeEvent(_sessionId, className:d.ClassName, packageName:d.PackageName));
//         }
//
//         private void _NewSession(string activityName)
//         {
//             _sessionId = SessionUtils.NewSessionId();
//             var count = _countRepository.AddMore(1);
//             SharedLogger.Log($"{Tag}->_NewSession: sessionId={_sessionId}, count={count}");
//             if (_tracking != null)
//             {
//                 var d = new JavaClassImpl(activityName);
//                 _tracking.Track(new SessionStartEvent(_sessionId, className: d.ClassName, packageName: d.PackageName));
//                 _tracking.SetProperty(PropertyConst.SESSION_ID, _sessionId);
//                 // var cachedData = _tracking.GetGameInterruptedData();
//                 // cachedData[PropertyConst.SESSION_ID] = _sessionId;
//                 // _tracking.PrepareGameInterruptedData(cachedData);
//             }
//
//             foreach (var handler in _newSessionHandlers) handler.Handle(count);
//         }
//     }
// }