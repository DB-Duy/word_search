using Shared.Core.IoC;
using Shared.Core.Repository.BoolType;

namespace Shared.Repository.Premium
{
    // = new BoolPlayerPrefsRepository(Constants.PlayerPrefsRepository.Premium, defaultValue: false);
    [Repository]
    public class PremiumEnableRepository : BoolPlayerPrefsRepository
    {
    }
}