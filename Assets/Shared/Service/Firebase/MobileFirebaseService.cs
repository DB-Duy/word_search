#if FIREBASE
using System;
using Firebase;
using Firebase.Analytics;
using Firebase.Crashlytics;
using Firebase.Extensions;
using Shared.Core.Async;
using Shared.Core.Handler;
using Shared.Core.IoC;
using Shared.Repository.Firebase;
using Shared.Service.Firebase.Handler;
using Shared.Utils;
using UnityEngine;
using Zenject;
using SystemInfo = UnityEngine.Device.SystemInfo;

namespace Shared.Service.Firebase
{
    [Service]
    public class MobileFirebaseService : IFirebaseService
    {
        private const string Tag = "MobileFirebaseService";

        [Inject] private FirebaseRepository _firebaseRepository;

        public IHandler<FirebaseApp> PostInitHandler { get; set; } = new MobileFirebaseInstallationIdPostInitHandler();
        public string AnalyticsInstanceId { get; private set; }
        
        public bool IsInitialized { get; private set; }
        
        private FirebaseApp _app;
        private IAsyncOperation _asyncOperation;
        
        public IAsyncOperation Initialize()
        {
            if (_asyncOperation != null) return _asyncOperation;
            _asyncOperation = new SharedAsyncOperation();
            FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>
            {
                SharedLogger.LogJson(SharedLogTag.Firebase, $"{Tag}->Execute.Result={task.Result}");
                IsInitialized = true;
                if (task.Result != DependencyStatus.Available)
                {
                    // Firebase Unity SDK is not safe to use here.
                    _asyncOperation.Fail($"task.Result={task.Result}");
                    return;
                }
                
                // When this property is set to true, Crashlytics will report all
                // uncaught exceptions as fatal events. This is the recommended behavior.
                Crashlytics.ReportUncaughtExceptionsAsFatal = true;
                
                // Create and hold a reference to your FirebaseApp, where app is a Firebase.FirebaseApp property of your application class.
                // Set a flag here to indicate whether Firebase is ready to use by your app.
                _app = FirebaseApp.DefaultInstance;
                FirebaseFlag.IsEnabled = true;
                
                // https://firebase.google.com/docs/analytics/unity/properties
                FirebaseAnalytics.SetUserId(SystemInfo.deviceUniqueIdentifier);
                FirebaseAnalytics.SetUserProperty("deviceRAM", SystemInfo.systemMemorySize.ToString());
                
                _RetrieveAnalyticsInstanceId();
                _asyncOperation.Success();
                PostInitHandler?.Handle(_app);
            });
            return _asyncOperation;
        }
        
        private void _RetrieveAnalyticsInstanceId()
        {
            try
            {
                FirebaseAnalytics.GetAnalyticsInstanceIdAsync().ContinueWithOnMainThread(task1 =>
                {
                    SharedLogger.LogInfoCustom(SharedLogTag.FirebaseNS2S, Tag, "FirebaseAnalytics.GetAnalyticsInstanceIdAsync()", nameof(task1.Result), task1.Result);
                    AnalyticsInstanceId = task1.Result;
                    var e = _firebaseRepository.Get();
                    e.AnalyticsInstanceId = AnalyticsInstanceId;
                    _firebaseRepository.Save(e);
                });
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }
    }
}
#endif