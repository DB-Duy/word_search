using Shared.Core.IoC;
using Shared.Core.Repository.JsonType;
using Shared.Entity.Ads;

namespace Shared.Repository.Ads
{
    [Repository]
    public class InterstitialAdRepository : JsonPlayerPrefsRepository<InterstitialAdEntity>
    {
    }
}