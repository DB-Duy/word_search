using Shared.Core.Handler;
using Shared.Core.IoC;
using Shared.Entity.Session;
using Shared.Service.Tracking;
using Shared.Tracking.Models.Session;
using Shared.Tracking.Property;
using Shared.Utils;

namespace Shared.Service.Session.Handler
{
    [Component]
    public class SessionCreatedTrackingHandler : IHandler<SessionEntity>, ISessionCreatedHandler, ISharedUtility
    {
        public void Handle(SessionEntity e)
        {
            this.LogInfo(nameof(e), e);
            this.Track(new SessionStartEvent(e.SessionId, className: e.NativeClass?.ClassName ?? "null", packageName: e.NativeClass?.PackageName ?? "null"));
            StaticEventPropertyRegistry.SetProperty(PropertyConst.SESSION_ID, e.SessionId);
        }
    }
}