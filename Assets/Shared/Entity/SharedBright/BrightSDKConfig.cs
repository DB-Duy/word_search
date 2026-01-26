using Newtonsoft.Json;

namespace Shared.Entity.SharedBright
{
    [System.Serializable]
    public class BrightSDKConfig
    {
        [JsonProperty("Unlocked")] public bool Unlocked { get; private set; }
        [JsonProperty("num_level_win")] public int NumLevelWin { get; private set; }
        [JsonProperty("num_show_each_session")] public int NumShowEachSession { get; private set; }

        // public static BrightSDKConfig NewClientDefault()
        // {
        //     var config = new BrightSDKConfig
        //     {
        //         _unlocked = true,
        //         _numLevelWin = 2,
        //         _numShowEachSession = 1
        //     };
        //     return config;
        // }

        // public override string ToString() => JsonConvert.SerializeObject(this);
    }
}

