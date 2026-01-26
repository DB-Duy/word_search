#if DEV_TO_DEV_MESSAGING && UNITY_IOS
using DevToDev.Messaging;
using DevToDev.Messaging.Platform;
using Shared.Common;
using Shared.Utils;

namespace Shared.SharedDevToDev.Messaging
{
    public class DevToDevIosMessagingController : IDevToDevMessagingController, IDTDPushListener
    {
        private const string Tag = "DevToDevIosMessagingController";
        private IAsyncOperation _initOperation;
        public bool IsInitialized { get; private set; } = false;
        public IAsyncOperation Initialize()
        {
            if (_initOperation != null) return _initOperation;
            DTDMessaging.IOS.SetNotificationOptions(DTDNotificationOptions.Alert | DTDNotificationOptions.Badge | DTDNotificationOptions.Sound);
            DTDMessaging.IOS.StartNotificationService();
            DTDMessaging.IOS.SetPushListener(this);
            _initOperation = new SharedAsyncOperation().Success();
            IsInitialized = true;
            return _initOperation;
        }

        public void OnPushServiceRegistrationSuccessful(string deviceId)
        {
            SharedLogger.Log($"{Tag}->OnPushServiceRegistrationSuccessful: {deviceId}");
        }

        public void OnPushServiceRegistrationFailed(string error)
        {
            SharedLogger.Log($"{Tag}->OnPushServiceRegistrationFailed: {error}");
        }

        public void OnPushNotificationReceived(DTDPushMessage message)
        {
            SharedLogger.Log($"{Tag}->OnPushNotificationReceived: {message}");
        }

        public void OnInvisibleNotificationReceived(DTDPushMessage message)
        {
            SharedLogger.Log($"{Tag}->OnInvisibleNotificationReceived: {message}");
        }

        public void OnPushNotificationOpened(DTDPushMessage pushMessage, DTDActionButton actionButton)
        {
            SharedLogger.Log($"{Tag}->OnPushNotificationOpened: pushMessage={pushMessage}, actionButton={actionButton}");
        }
    }
}
#endif