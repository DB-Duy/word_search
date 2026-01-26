#if UNITY_IOS && !UNITY_EDITOR

using Shared.Core.Async;
using Shared.Core.IoC;
using Shared.Repository.TrackingAuthorization;
using Shared.Utils;
using Zenject;

#if ADJUST
using AdjustSdk;
#endif

#if APPS_FLYER
using AppsFlyerSDK;
using Unity.Advertisement.IosSupport;
#endif

namespace Shared.Service.TrackingAuthorization
{
    /// <summary>
    /// App-tracking authorisation wrapper
    /// 
    /// Note: This feature exists only in iOS platform.
    /// Adjust SDK offers the possibility to use it for requesting user authorization in accessing their app-related entity.
    /// Adjust SDK has a wrapper built on top of the requestTrackingAuthorizationWithCompletionHandler: method, where you
    /// can as well define the callback method to get information about a user's choice. In order for this method to work,
    /// you need to specify a text which is going to be displayed as part of the tracking request dialog to your user.
    /// This setting is located inside of your iOS app Info.plist file under NSUserTrackingUsageDescription key.
    /// In case you don't want to add specify this on your own in your Xcode project, you can check Adjust prefab settings
    /// in inspector and specify this text under User Tracking Description.If specified there, Adjust iOS post-build process
    /// will make sure to add this setting into your app's Info.plist file.
    /// 
    /// Also, with the use of this wrapper, as soon as a user responds to the pop-up dialog, it's then communicated back
    /// using your callback method. The SDK will also inform the backend of the user's choice. The NSUInteger value will
    /// be delivered via your callback method with the following meaning:
    /// 
    /// 0: ATTrackingManagerAuthorizationStatusNotDetermined
    /// 1: ATTrackingManagerAuthorizationStatusRestricted
    /// 2: ATTrackingManagerAuthorizationStatusDenied
    /// 3: ATTrackingManagerAuthorizationStatusAuthorized
    /// </summary>
    [Service]
    public class IosTrackingAuthorizationService : ITrackingAuthorizationService, ISharedUtility
    {
        [Inject] private TrackingAuthorizationRepository _attRepository;
        
        public bool IsInitialized { get; private set; } = false;
        private IAsyncOperation _initOperation;

        public IAsyncOperation Initialize()
        {
            if (_initOperation != null) return _initOperation;
            _initOperation = new SharedAsyncOperation();
            
#if ADJUST
            Adjust.RequestAppTrackingAuthorization(_OnRequestAppTrackingAuthorization);
#endif
            
#if APPS_FLYER
            ATTrackingStatusBinding.AuthorizationTrackingStatus trackingStatus = ATTrackingStatusBinding.GetAuthorizationTrackingStatus();
            if (trackingStatus == ATTrackingStatusBinding.AuthorizationTrackingStatus.NOT_DETERMINED)
            {
                AppsFlyer.waitForATTUserAuthorizationWithTimeoutInterval(60);
                ATTrackingStatusBinding.RequestAuthorizationTracking(_OnRequestAppTrackingAuthorization);
            }
            else
            {
                _initOperation.Success();
            }
#endif
            return _initOperation;
        }

        private void _OnRequestAppTrackingAuthorization(int status)
        {
            _attRepository.Set(status);
            this.LogInfo(SharedLogTag.TrackingAuthorization, nameof(status), status);
            AttFlag.IsInitialized = true;
            IsInitialized = true;
            _initOperation.Success();
        }
    }
}
#endif