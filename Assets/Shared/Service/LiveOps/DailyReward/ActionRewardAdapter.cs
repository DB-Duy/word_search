using System;

namespace Shared.LiveOps.DailyReward
{
    public class ActionRewardAdapter : IRewardAdapter
    {
        private readonly string _rewardJsonString;
        private readonly Action<string> _action;

        public ActionRewardAdapter(string rewardJsonString, Action<string> action)
        {
            _rewardJsonString = rewardJsonString;
            _action = action;
        }
        
        public object AddReward()
        {
            _action.Invoke(_rewardJsonString);
            return _rewardJsonString;
        }
    }
}