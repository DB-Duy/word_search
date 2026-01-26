using Shared.Core.IoC;
using Shared.Core.Repository.RemoteConfig;
using Shared.Entity.StoreRating;

namespace Shared.Repository.StoreRating
{
    [Repository]
    public class StoreRatingConfigRepository : FirebaseRemoteConfigRepository<StoreRatingConfig>
    {
    }
}