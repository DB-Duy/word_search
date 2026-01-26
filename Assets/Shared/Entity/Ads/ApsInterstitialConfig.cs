namespace Shared.Entity.Ads
{
    [System.Serializable]
    public class ApsInterstitialConfig
    {
        public string StaticSlotId { get; private set; }
        public string VideoSlotId { get; private set; }

        public ApsInterstitialConfig(string staticSlotId, string videoSlotId)
        {
            StaticSlotId = staticSlotId;
            VideoSlotId = videoSlotId;
        }
    }
}