
using System;
using System.Collections.Generic;
using Shared.Core.Repository.JsonType;

namespace Shared.LiveOps.DailyReward
{
    public interface IDailyRewardController
    {
        IDailyRewardController SetUp(IRewardAdapter adapter, IJsonRepository<Dictionary<string, RewardEntity>> repository);
        IDailyRewardController AddValidators(params IExternalValidator[] validators);
        bool Validate();
        bool ValidateAndExecute(Action<IRewardAdapter> addAction = null, Action<IRewardAdapter> failedAction = null);
    }
}