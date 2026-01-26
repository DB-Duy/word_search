using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Shared.Core.Repository.JsonType;
using Shared.Utils;

namespace Shared.LiveOps.DailyReward
{
    public class DailyRewardController : IDailyRewardController
    {
        private IRewardAdapter _adapter;
        private IJsonRepository<Dictionary<string, RewardEntity>> _repository;
        private HashSet<IExternalValidator> _validators = new();
        
        public IDailyRewardController SetUp(IRewardAdapter adapter, IJsonRepository<Dictionary<string, RewardEntity>> repository)
        {
            _adapter = adapter;
            _repository = repository;
            return this;
        }

        public IDailyRewardController AddValidators(params IExternalValidator[] validators)
        {
            _validators.AddRange(validators);
            return this;
        }

        private void _SaveTodayReward(object reward)
        {
            var table = _repository.Get();
            var id = DailyRewardUtils.GetTodayId();
            if (table.ContainsKey(id)) throw new Exception($"id({id}) already existed in the table");
            string rawData;
            if (reward is string stringData) rawData = stringData;
            else rawData = JsonConvert.SerializeObject(reward);
            table.Add(id, new RewardEntity(id, rawData));
        }

        public bool Validate()
        {
            var id = DailyRewardUtils.GetTodayId();
            var table = _repository.Get();
            return !table.ContainsKey(id) && _validators.All(v => v.Validate());
        }

        public bool ValidateAndExecute(Action<IRewardAdapter> addAction = null, Action<IRewardAdapter> failedAction = null)
        {
            var validationResult = Validate();
            if (!Validate())
            {
                var rewardedObject = _adapter.AddReward();
                _SaveTodayReward(rewardedObject);
                addAction?.Invoke(_adapter);
                return validationResult;
            }
            failedAction?.Invoke(_adapter);
            return validationResult;
        }
    }
}