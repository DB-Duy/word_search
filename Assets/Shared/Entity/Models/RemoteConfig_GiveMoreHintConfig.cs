using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

namespace Shared.RemoteConfig
{
    [System.Serializable]
    public class GiveMoreHintConfig
    {
        [JsonProperty("level")] private List<int> _level;
        [JsonProperty("hint_give_away")] private List<int> _hint_give_away;

        [JsonIgnore] public List<int> Level => _level;
        [JsonIgnore] public List<int> Hint_give_away => _hint_give_away;

        public static GiveMoreHintConfig NewClientDefault()
        {
            GiveMoreHintConfig config = new GiveMoreHintConfig();
            config._level = new List<int> { 13, 14, 15 };
            config._hint_give_away = new List<int> { 0, 0, 1, 2, 3 };
            return config;
        }

        public override string ToString() => JsonConvert.SerializeObject(this);
    }
}

