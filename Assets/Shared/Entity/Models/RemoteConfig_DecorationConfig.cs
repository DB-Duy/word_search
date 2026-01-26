using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

namespace Shared.RemoteConfig
{
    [System.Serializable]
    public class DecorationConfig
    {
        [JsonProperty("unlock")] private bool _unlock;
        [JsonProperty("first_level")] private int _first_level;
        [JsonProperty("dictDecorConfig")] Dictionary<int, DecorationData> _dictDecorConfig;

        [JsonIgnore] public bool unlock => _unlock;
        [JsonIgnore] public int first_level => _first_level;
        [JsonIgnore] public Dictionary<int, DecorationData> dictDecorConfig => _dictDecorConfig;


        public static DecorationConfig NewClientDefault()
        {
            DecorationConfig decorationConfig = new DecorationConfig();
            decorationConfig._unlock = true;
            decorationConfig._first_level = 2;

            string jsonString = Resources.Load<TextAsset>("Jsons/decorConfig").text;
            Dictionary<int, DecorationData> datas = JsonConvert.DeserializeObject<Dictionary<int, DecorationData>>(jsonString);
            decorationConfig._dictDecorConfig = datas;

            return decorationConfig;
        }

        public override string ToString() => JsonConvert.SerializeObject(this);
    }

    [System.Serializable]
    public class DecorationData
    {
        private int _id;
        private int _level_unlock;
        private int _total_color;
        private string _lives_per_color;

        public int id { get => _id; set => _id = value; }
        public int level_unlock { get => _level_unlock; set => _level_unlock = value; }
        public int total_color { get => _total_color; set => _total_color = value; }
        public string lives_per_color { get => _lives_per_color; set => _lives_per_color = value; }
    }

}