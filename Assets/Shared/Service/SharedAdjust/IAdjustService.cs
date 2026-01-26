using Shared.Core.Handler.Corou.Initializable;

namespace Shared.Service.SharedAdjust
{
    public interface IAdjustService : IInitializable
    {
#if ADJUST
        Shared.Entity.SharedAdjust.AdjustEntity Get();
#endif
    }
}