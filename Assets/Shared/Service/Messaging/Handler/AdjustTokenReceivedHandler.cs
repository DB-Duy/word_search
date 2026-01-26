#if HUAWEI && ADJUST
using com.adjust.sdk;
using Shared.Utils;

namespace Shared.SharedMessaging
{
    public class AdjustTokenReceivedHandler : IMessagingTokenReceivedHandler
    {
        private const string TAG = "AdjustTokenReceivedHandler";
        
        public void Handle(string token)
        {
            SharedLogger.Log($"{TAG}->Handle: Adjust.isEnabled()={Adjust.isEnabled()}");
            Adjust.setDeviceToken(token);
        }
    }
}
#endif