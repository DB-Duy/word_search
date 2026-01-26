#if LEVEL_PLAY && USING_UMP
using System.Collections;
using Shared.Core.IoC;
using Shared.Service.Ump;
using Shared.Utils;
using Zenject;

namespace Shared.Service.Ads.SharedLevelPlay.PostHandler
{
    [Component]
    public class LevelPlayPostInitUmpHandler : ILevelPlayPostInitHandler, ISharedUtility
    {
        [Inject] private IUmpService _umpService;
        
        public IEnumerator Handle()
        {
            this.LogInfo(SharedLogTag.AdNLevelPlayNUmp);
            _umpService.Sync();
            yield break;
        }
    }
}
#endif