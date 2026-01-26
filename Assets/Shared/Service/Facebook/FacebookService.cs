#if FACEBOOK_SDK
using System.Diagnostics;
using Facebook.Unity;
using Shared.Core.Async;
using Shared.Core.IoC;
using Shared.Entity.Config;
using Shared.Repository.TrackingAuthorization;
using Shared.Service.TrackingAuthorization;
using Shared.Utilities;
using Shared.Utils;
using Zenject;

namespace Shared.Service.Facebook
{
    [Service]
    public class FacebookService : IFacebookService, ISharedLogTag, ISharedUtility
    {
        public string LogTag => SharedLogTag.Facebook;
        
#if UNITY_IOS
        [System.Runtime.InteropServices.DllImport("__Internal")]
        private static extern void FBAdSettingsBridgeSetAdvertiserTrackingEnabled(bool advertiserTrackingEnabled);
#endif
        
        [Inject] private IConfig _config;
        [Inject] private TrackingAuthorizationRepository _attRepository;

        public bool IsInitialized => FB.IsInitialized;
        private IAsyncOperation _initOperation;

        public IAsyncOperation Initialize()
        {
            if (_initOperation != null) return _initOperation;
            _initOperation = new SharedAsyncOperation();
            _attRepository.onIntValueUpdated.AddListener(_OnTrackingAuthorizationRepositoryValueChanged);
            if (!FB.IsInitialized)
            {
                _SetupAdvertiserTrackingEnabled();
                if (string.IsNullOrEmpty(_config.FacebookId))
                {
                    this.LogInfo("call", "FB.Init(onInitComplete: _OnInitComplete);");
                    FB.Init(onInitComplete: _OnInitComplete);
                }
                else
                {
                    this.LogInfo("call", $"FB.Init(appId: {_config.FacebookId}, clientToken: {_config.FacebookClientToken}, onInitComplete: _OnInitComplete);");
                    FB.Init(appId: _config.FacebookId, clientToken: _config.FacebookClientToken, onInitComplete: _OnInitComplete);
                }
            }
            else
            {
                this.LogInfo("call", "FB.ActivateApp();");
                FB.ActivateApp();
            }

            return _initOperation;
        }

        private void _OnInitComplete()
        {
            if (FB.IsInitialized)
            {
                this.LogInfo(nameof(FB.AppId), FB.AppId, nameof(FB.IsInitialized), FB.IsInitialized);
                FB.ActivateApp();
                _initOperation.Success();
                _SetupAdvertiserTrackingEnabled();
            }
            else
            {
                this.LogInfo(nameof(FB.IsInitialized), FB.IsInitialized);
                _initOperation.Fail($"{nameof(FB.IsInitialized)}=false");
            }
        }

        [Conditional("UNITY_IOS")]
        private void _SetupAdvertiserTrackingEnabled()
        {
#if UNITY_IOS && !UNITY_EDITOR
            var v = _attRepository.Get();
            if (v < 0) return;
            var status = (TrackingAuthorizationStatus) _attRepository.Get();
            var isAttAllowed = status == TrackingAuthorizationStatus.Authorized;
            FBAdSettingsBridgeSetAdvertiserTrackingEnabled(isAttAllowed);
            this.LogInfo(SharedLogTag.Facebook, nameof(isAttAllowed), isAttAllowed, "call", $"FBAdSettingsBridgeSetAdvertiserTrackingEnabled({isAttAllowed});)");
#endif
        }
        
        private void _OnTrackingAuthorizationRepositoryValueChanged(int oldValue, int newValue)
        {
            this.LogInfo(nameof(newValue), newValue, nameof(oldValue), oldValue);
            _SetupAdvertiserTrackingEnabled();
        }
    }
}
#endif