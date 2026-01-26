namespace Shared.Service.Tracking.Common
{
    public class AdUnit : ValueObject
    {
        private const string KInterstitial = "interstitial";
        private const string KBanner = "banner";
        private const string KMrec = "mrec";
        private const string KRewarded = "rewarded";
        private const string KAudio = "audio";
        private const string KInPlay = "inplay";
        
        private AdUnit(string v) : base(v)
        {
        }

        public bool Equals(string c) => c == _v;

        public bool IsInterstitial() => _v == KInterstitial;
        public bool IsBanner() => _v == KBanner;
        public bool IsRewarded() => _v == KRewarded;
        public bool IsAudio() => _v == KAudio;

        public static readonly AdUnit Interstitial = new(KInterstitial);
        public static readonly AdUnit Banner = new(KBanner);
        public static readonly AdUnit Rewarded = new(KRewarded);
        public static readonly AdUnit Audio = new(KAudio);
        public static readonly AdUnit Inplay = new(KInPlay);
        public static readonly AdUnit Mrec = new(KMrec);
    }
}