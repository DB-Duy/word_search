#if GOOGLE_PLAY
using Newtonsoft.Json;
using Shared.Core.Async;
using Shared.Core.Repository.JsonType;
using Shared.Service.Tracking;
using Shared.Tracking;
using Shared.Tracking.Models.Game;
using Shared.Utils;
using UnityEngine;

namespace Shared.Referrer
{
    /// <summary>
    /// https://ga-dev-tools.google/ga4/campaign-url-builder/play/
    /// https://proandroiddev.com/play-install-referrer-library-android-b99536530ca4
    /// 
    /// {
    ///     "installBeginTimestampSeconds": 1718557911,
    ///     "googlePlayInstantParam": false,
    ///     "installBeginTimestampServerSeconds": 1718557913,
    ///     "referrerClickTimestampServerSeconds": 0,
    ///     "installReferrer": "utm_source=google-play&amp;utm_medium=organic",
    ///     "referrerClickTimestampSeconds": 0,
    ///     "installVersion": "1.1.0"
    /// }
    ///
    /// {
    ///     "installBeginTimestampSeconds": 1720663985,
    ///     "googlePlayInstantParam": false,
    ///     "installBeginTimestampServerSeconds": 1720663985,
    ///     "referrerClickTimestampServerSeconds": 1720663939,
    ///     "installReferrer": "utm_source=admob&amp;utm_campaign=first_campaign&amp;anid=admob",
    ///     "referrerClickTimestampSeconds": 1720663939,
    ///     "installVersion": "1.2.0"
    /// }
    /// 
    /// </summary>
    public class GooglePlayInstallReferrerController : IInstallReferrerController, ISharedUtility
    {
        private const string TAG = "GooglePlayInstallReferrerController";
        private const string JavaClassName = "com.unity3d.player.InstallReferrerController";

        private AndroidJavaClass _javaClass;
        private AndroidJavaObject _javaObject;

        public bool IsInitialized => _initAsyncOperation is { IsSuccess: true };
        private IAsyncOperation _initAsyncOperation;

        public ReferrerDetails ReferrerDetails { get; private set; }
        public InstallReferrer InstallReferrer { get; private set; }
        private IJsonRepository<ReferrerDetails> _referrerDetailsRepository; 
        
        public IInstallReferrerController Setup(IJsonRepository<ReferrerDetails> repository)
        {
            _referrerDetailsRepository = repository;
            return this;
        }
        
        public IAsyncOperation Initialize()
        {
            SharedLogger.Log($"{TAG}->Initialize");
            if (Application.isEditor) return _initAsyncOperation ??= new SharedAsyncOperation().Success();
            if (_initAsyncOperation != null) return _initAsyncOperation;

             ReferrerDetails = _referrerDetailsRepository?.Get();
            if (ReferrerDetails != null && !string.IsNullOrEmpty(ReferrerDetails.InstallReferrer)) InstallReferrer = InstallReferrer.NewInstance(ReferrerDetails.InstallReferrer);
            
            _javaClass = new AndroidJavaClass(JavaClassName);
            _javaObject = _javaClass.CallStatic<AndroidJavaObject>("getInstance");
            var callback = new UnityInstallReferrerCallback();
            callback.OnInstallReferrerSetupFinishedEvent += _OnInstallReferrerSetupFinishedEvent;
            callback.OnInstallReferrerServiceDisconnectedEvent += _OnInstallReferrerServiceDisconnectedEvent;
            _javaObject.Call("setUnityInstallReferrerCallback", callback);
            _javaObject.Call("connect");

            _initAsyncOperation = new SharedAsyncOperation();
            return _initAsyncOperation;
        }

        private void _OnInstallReferrerSetupFinishedEvent(int code)
        {
            SharedLogger.Log($"{TAG}->_OnInstallReferrerSetupFinishedEvent: {code}");
            if (code == InstallReferrerResponse.OK)
            {
                // {"installBeginTimestampSeconds":1718557911,"googlePlayInstantParam":false,"installBeginTimestampServerSeconds":1718557913,"referrerClickTimestampServerSeconds":0,"installReferrer":"utm_source=google-play&utm_medium=organic","referrerClickTimestampSeconds":0,"installVersion":"1.1.0"}
                var data = _javaObject.Call<string>("getInstallReferrer");
                SharedLogger.LogProduction($"{TAG}->_OnInstallReferrerSetupFinishedEvent: {data}");
                _javaObject.Call("close");
                ReferrerDetails = JsonConvert.DeserializeObject<ReferrerDetails>(data);
                if (!string.IsNullOrEmpty(ReferrerDetails.InstallReferrer)) InstallReferrer = InstallReferrer.NewInstance(ReferrerDetails.InstallReferrer);
                _referrerDetailsRepository?.Save(ReferrerDetails);
                _initAsyncOperation?.Success();
            }
            else
            {
                _initAsyncOperation?.Fail($"code={code}");
            }
        }

        private void _OnInstallReferrerServiceDisconnectedEvent()
        {
            SharedLogger.Log($"{TAG}->_OnInstallReferrerServiceDisconnectedEvent");
        }

        public void TrackReferrer()
        {
            if (Application.isEditor) return;
            if (ReferrerDetails == null)
            {
                SharedLogger.LogError($"{TAG}->Track: _referrerDetails == null");
                return;
            }
            
            var trackingParams = new InstallReferrerParams
            {
                GooglePlayInstantParam = ReferrerDetails.GooglePlayInstantParam,
                InstallReferrer = ReferrerDetails.InstallReferrer,
                InstallBeginTimestampSeconds = ReferrerDetails.InstallBeginTimestampSeconds,
                InstallBeginTimestampServerSeconds = ReferrerDetails.InstallBeginTimestampServerSeconds,
                InstallVersion = ReferrerDetails.InstallVersion,
                ReferrerClickTimestampSeconds = ReferrerDetails.ReferrerClickTimestampSeconds,
                ReferrerClickTimestampServerSeconds = ReferrerDetails.ReferrerClickTimestampServerSeconds
            };
            
            this.Track(trackingParams);
        }
    }
}
#endif