using Shared.Core.IoC;
using Shared.Core.Repository.IntType;

namespace Shared.Repository.SharedBright
{
    // IntPlayerPrefsRepository(Constants.PlayerPrefsRepository.HintRewardUsed, defaultValue: 0);
    [Repository]
    public class HintRewardUsedRepository : IntStoreRepository
    {
    }
}