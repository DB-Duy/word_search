#if TOPON
using Shared.Ads.SharedTopOn.PreHandler;

namespace Shared.Ads.SharedTopOn
{
    public interface ITopOnAdController : IAdController
    {
        ITopOnAdController AddPreInitHandlers(params IPreInitHandler[] handlers);
        ITopOnAdController AddImpressionHandlers(params ITopOnImpressionDataReadyEventHandler[] handlers);
    }
}
#endif