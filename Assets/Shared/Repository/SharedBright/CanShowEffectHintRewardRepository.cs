using Shared.Core.IoC;
using Shared.Core.Repository.BoolType;

namespace Shared.Repository.SharedBright
{
    // new BoolPlayerPrefsRepository(Constants.PlayerPrefsRepository.HintRewardEffect, defaultValue: true);
    [Repository]
    public class CanShowEffectHintRewardRepository : BoolPlayerPrefsRepository
    {
    }
}