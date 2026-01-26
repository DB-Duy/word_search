#if LEVEL_PLAY
using System.Collections.Generic;
using System.Globalization;
using Shared.Service.Ads.Common;

namespace Shared.Service.Ads.SharedLevelPlay.FS
{
    public static class LevelPlayInterstitialExtensions
    {
        const string Tag = "Extensions";
        
        public static void DestroyAd(this List<LevelPlayInterstitialAdWrapper> l)
        {
            foreach (var i in l) i.DestroyAd();
        }
            
        public static void Recreate(this List<LevelPlayInterstitialAdWrapper> l)
        {
            foreach (var i in l) i.Create();
        }

        public static void LoadAd(this List<LevelPlayInterstitialAdWrapper> l)
        {
            foreach (var i in l) i.LoadAd();
        }

        public static LevelPlayInterstitialAdWrapper ShowAd(this List<LevelPlayInterstitialAdWrapper> l, IAdPlacement placement)
        {
            var bestOne = l[0];
            for (var i = 1; i < l.Count; i++)
            {
                if (l[i].CachedAdInfo == null) continue;
                var oldRev = bestOne.CachedAdInfo.Revenue ?? 0;
                var newRev = l[i].CachedAdInfo.Revenue ?? 0;
                if (oldRev < newRev) bestOne = l[i];
            }

            bestOne?.ShowAd(placement);
            return bestOne;
        }
        
        public static Dictionary<string, string> ToRevenueMap(this List<LevelPlayInterstitialAdWrapper> l)
        {
            var revenue = new Dictionary<string, string>();
            foreach (var i in l)
            {
                var v = i.CachedAdInfo == null ? "null" : (i.CachedAdInfo.Revenue ?? 0).ToString(CultureInfo.InvariantCulture);
                revenue.Add(i.Id,  v);
            }
            return revenue;
        }
    }
}
#endif