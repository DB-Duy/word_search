#if ADJUST
using AdjustSdk;
using Shared.Core.Handler;

namespace Shared.Service.SharedAdjust.Handler
{
    public interface IAdjustAttributionHandler :  IHandler<AdjustAttribution>, ISharedUtility
    {
        
    }
}
#endif