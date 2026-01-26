using Shared.Core.IoC;
using Shared.Core.Repository.IntType;

namespace Shared.Repository.StoreRating
{
    // new IntPlayerPrefsRepository(Constants.PlayerPrefsRepository.FakeRating, defaultValue: 0);
    [Repository]
    public class StoreRatingShownCountRepository : IntStoreRepository
    {
    }
}