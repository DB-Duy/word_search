#if DEV_TO_DEV_MESSAGING && UNITY_ANDROID
using DevToDev.Messaging;
using DevToDev.Messaging.Platform;
using Shared.Common;
using Shared.Utils;

namespace Shared.SharedDevToDev.Messaging
{
    public class DevToDevAndroidMessagingController : IDevToDevMessagingController, IDTDPushListener
    {
        private const string Tag = "DevToDevAndroidMessagingController";
        
        public bool IsInitialized { get; private set; }
        
        public IAsyncOperation Initialize()
        {
            if (IsInitialized) return new SharedAsyncOperation().Success();
            IsInitialized = true;
            DTDMessaging.Android.SetPushListener(this);
            DTDMessaging.Android.Initialize();
            DTDMessaging.Android.StartPushService();
            return new SharedAsyncOperation().Success();
        }

        public void OnInvisibleNotificationReceived(DTDPushMessage message)
        {
            //IOS only.
            SharedLogger.Log($"{Tag}->OnInvisibleNotificationReceived: {message}");
        }

        public void OnPushNotificationOpened(DTDPushMessage pushMessage, DTDActionButton actionButton)
        {
            SharedLogger.Log($"{Tag}->OnPushNotificationOpened: {pushMessage} {actionButton}");
        }

        public void OnPushNotificationReceived(DTDPushMessage message)
        {
            SharedLogger.Log($"{Tag}->OnPushNotificationReceived: {message}");
        }

        public void OnPushServiceRegistrationFailed(string error)
        {
            SharedLogger.Log($"{Tag}->OnPushServiceRegistrationFailed: {error}");
        }

        public void OnPushServiceRegistrationSuccessful(string deviceId)
        {
            SharedLogger.Log($"{Tag}->OnPushServiceRegistrationSuccessful: {deviceId}");
        }
    }
}
#endif