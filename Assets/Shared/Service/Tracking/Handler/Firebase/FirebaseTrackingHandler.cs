#if FIREBASE

using Shared.Core.IoC;
using Shared.Service.Firebase;
using Shared.Tracking.Templates;
using Shared.Utils;

namespace Shared.Service.Tracking.Handler.Firebase
{
    [Component]
    public class FirebaseTrackingHandler : ITrackingHandler, ISharedUtility
    {
        public void Handle(ITrackingEvent e)
        {
            if (!FirebaseFlag.IsEnabled) return;
            if (e is not IConvertableEvent ee) return;
            this.LogInfo(SharedLogTag.TrackingNFirebase, nameof(e.EventName), e.EventName);
            global::Firebase.Analytics.FirebaseAnalytics.LogEvent(name: e.EventName, parameters: ee.ToFirebaseParams());
        }
        
        
    }
}
#endif