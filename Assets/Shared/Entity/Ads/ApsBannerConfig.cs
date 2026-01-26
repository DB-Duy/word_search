using Newtonsoft.Json;

namespace Shared.Entity.Ads
{
    [System.Serializable]
    public class ApsBannerConfig
    {
        [JsonProperty("w")] public int Width { get; }
        [JsonProperty("h")] public int Height { get; }
        [JsonProperty("id")] public string Id { get; }

        public ApsBannerConfig(int width, int height, string id)
        {
            Width = width;
            Height = height;
            Id = id;
        }

        public override string ToString() => JsonConvert.SerializeObject(this);

        public static ApsBannerConfig NewPhoneInstance(string id) => new(320, 50, id);
        public static ApsBannerConfig NewTabletInstance(string id) => new(728, 90, id);
    }
}