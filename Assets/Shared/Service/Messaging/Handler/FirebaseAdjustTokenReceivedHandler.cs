#if ADJUST && FIREBASE
using AdjustSdk;
using Shared.Core.IoC;
using Shared.Utils;

namespace Shared.Service.Messaging.Handler
{
    [Component]
    public class FirebaseAdjustTokenReceivedHandler : IMessagingTokenReceivedHandler, ISharedUtility
    {
        public void Handle(string token)
        {
            this.LogInfo(SharedLogTag.FirebaseNMessaging,nameof(token), token);
            if (string.IsNullOrEmpty(token)) return; 
            Adjust.SetPushToken(token);
        }
    }
}
#endif