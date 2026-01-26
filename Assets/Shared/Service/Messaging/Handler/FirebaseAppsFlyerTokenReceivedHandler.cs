#if APPS_FLYER && FIREBASE
using AppsFlyerSDK;
using Shared.Core.IoC;
using Shared.Utils;

namespace Shared.Service.Messaging.Handler
{
    /// <summary>
    /// https://dev.appsflyer.com/hc/docs/uninstallmeasurement
    /// </summary>
    [Component]
    public class FirebaseAppsFlyerTokenReceivedHandler : IMessagingTokenReceivedHandler, ISharedUtility
    {
        public void Handle(string token)
        {
            this.LogInfo(SharedLogTag.FirebaseNMessagingNAppsFlyer,nameof(token), token, "forUninstallMeasurement", "Uninstall Measurement");
            if (string.IsNullOrEmpty(token)) return; 
            AppsFlyer.updateServerUninstallToken(token);
        }
    }
}
#endif