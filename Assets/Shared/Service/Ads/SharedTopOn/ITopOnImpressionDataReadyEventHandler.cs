#if TOPON
using AnyThinkAds.Api;

namespace Shared.Ads.SharedTopOn
{
    public interface ITopOnImpressionDataReadyEventHandler
    {
        void Handle(ATAdEventArgs entity);
    }
}
#endif