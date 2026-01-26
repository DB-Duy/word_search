#if ADVERTY && ADVERTY_4
using Adverty;
using Shared.Core.Handler;
using Shared.Core.IoC;
using Shared.Entity.Ump;
using Shared.Utils;

namespace Shared.Service.Ump.Handlers
{
    [Component]
    public class AdvertyUmpValueSyncHandler : IHandler<UmpEntity>, ISharedUtility, IUmpValueSyncHandler
    {
        public void Handle(UmpEntity e)
        {
            var userData = new UserData(AgeSegment.Unknown, Gender.Unknown, e.TcString);
            AdvertySDK.UpdateUserData(userData);
            this.LogInfo(SharedLogTag.UmpNAdverty, nameof(e), e, nameof(userData), userData);
        }
    }
}
#endif