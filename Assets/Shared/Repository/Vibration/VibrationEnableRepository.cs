using Shared.Core.IoC;
using Shared.Core.Repository.BoolType;

namespace Shared.Repository.Vibration
{
    // new BoolPlayerPrefsRepository(Constants.PlayerPrefsRepository.Vibration, defaultValue: true);
    [Repository]
    public class VibrationEnableRepository : BoolPlayerPrefsRepository
    {
    }
}