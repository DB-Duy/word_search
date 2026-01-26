#if FIREBASE
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Firebase.Extensions;
using Firebase.RemoteConfig;
using Shared.Core.Async;
using Shared.Core.Handler;
using Shared.Core.IoC;
using Shared.Service.Firebase;
using Shared.Service.FirebaseRemoteConfig.Handler;
using Shared.Service.SharedCoroutine;
using Shared.Utils;

namespace Shared.Service.FirebaseRemoteConfig
{
    /// <summary>
    /// https://firebase.google.com/docs/remote-config/get-started?platform=unity#add-real-time-listener
    /// https://firebase.google.com/docs/remote-config/real-time?platform=unity#real-time-client-server-connection
    /// </summary>
    [Service]
    public class MobileFirebaseRemoteConfigService : IFirebaseRemoteConfigService, ISharedUtility
    {
        private const string Tag = "MobileFirebaseRemoteConfigService";

        public IHandler<Dictionary<string, string>> RemoteConfigChangeHandler { get; } = new AbTestHandler();

        private IAsyncOperation _initOperation;
        private IAsyncOperation _asyncOperation;
        private bool _allowProcessAbTestingProcessing = false;

        public bool IsInitialized => _initOperation != null && _initOperation.IsComplete && _initOperation.IsSuccess;
        
        public IAsyncOperation Initialize()
        {
            if (_initOperation != null) return _initOperation;
            _initOperation = new SharedAsyncOperation();
            this.StartSharedCoroutine(_Initialize());
            return _initOperation;
        }

        private IEnumerator _Initialize()
        {
            while (!FirebaseFlag.IsEnabled) yield return null;
            global::Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance.OnConfigUpdateListener += _OnConfigUpdateListener;
            var fetchOperation = Fetch();
            while (!fetchOperation.IsComplete) yield return null;
            if (fetchOperation.IsSuccess) 
                _initOperation.Success();
            else
                _initOperation.Fail(fetchOperation.FailReason);
        }

        public IAsyncOperation Fetch()
        {
            if (_asyncOperation != null && !_asyncOperation.IsComplete) return _asyncOperation;
            _asyncOperation = new SharedAsyncOperation();
            Task.WhenAll(_FetchRemoteConfigDataTask());
            return _asyncOperation;
        }

        // Start a fetch request.
        // FetchAsync only fetches new entity if the current entity is older than the provided
        // timespan.  Otherwise it assumes the entity is "recent enough", and does nothing.
        // By default the timespan is 12 hours, and for production apps, this is a good
        // number. For this example though, it's set to a timespan of zero, so that
        // changes in the console will always show up immediately.
        private Task _FetchRemoteConfigDataTask()
        {
            SharedLogger.LogJson(SharedLogTag.FirebaseNConfig, $"{Tag}->_FetchRemoteConfigDataTask");
            var fetchTask = global::Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance.FetchAsync(TimeSpan.Zero);
            return fetchTask.ContinueWithOnMainThread(_FetchComplete);
        }

        private void _FetchComplete(Task fetchTask)
        {
            SharedLogger.LogJson(SharedLogTag.FirebaseNConfig, $"{Tag}->_FetchComplete");
            ConfigInfo info;
            try
            {
                info = global::Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance.Info;
                SharedLogger.LogJson(SharedLogTag.FirebaseNConfig, $"{Tag}->_FetchComplete", nameof(info), info);
            }
            catch (Exception e)
            {
                SharedLogger.LogJson(SharedLogTag.FirebaseNConfig, $"{Tag}->_FetchComplete", nameof(e), e.Message);
                _asyncOperation.Fail($"Get ConfigInfo Error");
                return;
            }
            
            if (info.LastFetchStatus == LastFetchStatus.Success)
            {
                global::Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance.ActivateAsync().ContinueWith((a) =>
                {
                    _asyncOperation.Success();
                    _ProcessAbTestDetector();
                });
            }
            else
            {
                _asyncOperation.Fail($"info.LastFetchStatus={info.LastFetchStatus.ToString()}");
            }
        }

        private void _OnConfigUpdateListener(object sender, ConfigUpdateEventArgs args)
        {
            if (args.Error != RemoteConfigError.None) {
                this.LogInfo(SharedLogTag.FirebaseNConfig, nameof(args.Error), args.Error);
                return;
            }
            this.LogInfo(SharedLogTag.FirebaseNConfig, nameof(args.UpdatedKeys), args.UpdatedKeys);
            // Activate all fetched values and then display a welcome message.
            global::Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance.ActivateAsync().ContinueWith((a) =>
            {
                _ProcessAbTestDetector();
                foreach (var updatedKey in args.UpdatedKeys)
                    FirebaseRemoteConfigRegistry.OnValueChanged(updatedKey);
            });
        }
        
        public void ProcessAbTestingDetector()
        {
            _allowProcessAbTestingProcessing = true;
            _ProcessAbTestDetector();
        }

        private void _ProcessAbTestDetector()
        {
            if (!_allowProcessAbTestingProcessing)
            {
                this.LogInfo("ingore", "!_allowProcessAbTestingProcessing");
                return;
            }

            if (!FirebaseFlag.IsEnabled)
            {
                this.LogInfo("ingore", "!FirebaseFlag.IsEnabled");
                return;
            }

            SharedLogger.LogJson(SharedLogTag.FirebaseNConfig, $"{Tag}->_ProcessAbTestDetector");
            
            var allConfig = global::Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance.AllValues;
            Dictionary<string, string> filteredDict = new();
            foreach (var c in allConfig)
            {
                var stringValue = c.Value.StringValue.Trim();
                SharedLogger.LogJson(SharedLogTag.FirebaseNConfig, $"{Tag}->_ProcessAbTestDetector", nameof(c.Key), c.Key, nameof(c.Value.StringValue), c.Value.StringValue);
                if (stringValue.StartsWith("{") && stringValue.EndsWith("}")) filteredDict.Add(c.Key, stringValue);
            }
            RemoteConfigChangeHandler.Handle(filteredDict);
        }
    }
}
#endif