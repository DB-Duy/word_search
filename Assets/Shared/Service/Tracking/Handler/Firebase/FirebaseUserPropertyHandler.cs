#if FIREBASE
using Firebase.Analytics;
using Shared.Core.Handler;
using Shared.Core.IoC;
using Shared.Service.Firebase;
using Shared.Utils;

namespace Shared.Service.Tracking.Handler.Firebase
{
    [Component]
    public class FirebaseUserPropertyHandler : IHandler<string, object>, ISharedUtility, IUserPropertyHandler
    {
        public void Handle(string k, object v)
        {
            if (!FirebaseFlag.IsEnabled) return;
            var fv = v.ToString();
            this.LogInfo(SharedLogTag.UserPropertyNFirebase, nameof(k), k, nameof(fv), fv);
            FirebaseAnalytics.SetUserProperty(k, fv);
        }
    }
}
#endif