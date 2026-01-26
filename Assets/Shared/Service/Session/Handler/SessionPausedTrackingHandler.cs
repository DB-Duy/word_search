using Shared.Core.Handler;
using Shared.Core.IoC;
using Shared.Entity.Session;
using Shared.Service.Tracking;
using Shared.Tracking.Models.Session;
using Shared.Utils;

namespace Shared.Service.Session.Handler
{
    [Component]
    public class SessionPausedTrackingHandler : IHandler<SessionEntity, NativeClassEntity>, ISharedUtility, ISessionPausedHandler
    {
        public void Handle(SessionEntity e, NativeClassEntity n)
        {
            this.LogInfo(nameof(e), e);
            this.Track(new SessionPauseEvent(e.SessionId, className: n != null ? n.ClassName ?? "null" : "null", packageName: n != null ? n.PackageName ?? "null" : "null"));
        }
    }
}