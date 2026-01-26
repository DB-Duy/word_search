using Newtonsoft.Json;
using System.Collections.Generic;

namespace Shared.Entity.ParentControl
{
    public class ParentControlConfig
    {
        [JsonProperty("Unlocked")] public bool Unlocked { get; private set; }
  
        public override string ToString() => JsonConvert.SerializeObject(this);
    }
}