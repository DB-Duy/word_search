using Newtonsoft.Json;

namespace Shared.Entity.ParentControl
{
    [System.Serializable]
    public class ParentControlStepEntity
    {
        [JsonProperty("step")] public Step Step { get; set; }

        public ParentControlStepEntity()
        {}

        public ParentControlStepEntity(Step step) => Step = step;

        public override string ToString() => JsonConvert.SerializeObject(this);
    }
}