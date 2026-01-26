#if LEVEL_PLAY
using Shared.Core.Handler;
using Shared.Core.IoC;
using Shared.Entity.Ump;
using Shared.Service.Ads.SharedLevelPlay;
using Shared.Utils;

namespace Shared.Service.Ump.Handlers
{
    [Component]
    public class LevelPlayUmpValueSyncHandler : IHandler<UmpEntity>, ISharedUtility, IUmpValueSyncHandler
    {
        public void Handle(UmpEntity e)
        {
            if (!e.GdprApply)
            {
                this.LogInfo(SharedLogTag.UmpNLevelPlay, nameof(e.GdprApply), e.GdprApply);
                return;
            }
            IronSource.Agent.setMetaDataWithLog("do_not_sell", e.UsPrivacyString.ToDoNotSellString());
            this.LogInfo(SharedLogTag.UmpNLevelPlay, "do_not_sell", e.UsPrivacyString.ToDoNotSellString());
        }
    }
}
#endif