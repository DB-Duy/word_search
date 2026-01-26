using Shared.Core.Handler.Async;
using Shared.Entity.Ads;

namespace Shared.Service.Ads.SharedLevelPlay.Banner
{
    public interface IApsBannerFetcher : IAsyncHandler<ApsBannerConfig>
    {
    }
}