using System;
using Newtonsoft.Json;

namespace Shared.Entity.ParentControl
{
    [System.Serializable]
    public class ParentControlEntity
    {
        [JsonProperty("yearOfBirth")] public int YearOfBirth { get; set; }
        [JsonProperty("gender")] public Gender Gender { get; set; }
        
        [JsonIgnore] public int Age => DateTime.Now.Year - YearOfBirth;
    }
}