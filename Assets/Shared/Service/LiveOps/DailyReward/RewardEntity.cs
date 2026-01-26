using Newtonsoft.Json;

namespace Shared.LiveOps.DailyReward
{
    [System.Serializable]
    public class RewardEntity
    {
        [JsonProperty("id")] public string Id { get; }
        [JsonProperty("reward")] public string Reward { get; }

        public RewardEntity(string id, string reward)
        {
            Id = id;
            Reward = reward;
        }

        public override string ToString() => JsonConvert.SerializeObject(this);
    }
}