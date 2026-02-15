
using System;
using Newtonsoft.Json;

namespace Entity.UserData
{
    [Serializable]
    public class UserDataEntity
    {  
        [JsonProperty("level")] public int Level { get; set; }
    }
}