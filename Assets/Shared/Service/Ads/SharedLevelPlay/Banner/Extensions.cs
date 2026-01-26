#if LEVEL_PLAY
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Shared.Utils;

namespace Shared.Service.Ads.SharedLevelPlay.Banner
{
    public static class Extensions
    {
        const string Tag = "Extensions";
        public static void LoadBannerAd(this List<LevelPlayBannerWrapper> l)
        {
            foreach (var i in l) i.LoadBannerAd();
        }
        
        public static void DestroyBannerAd(this List<LevelPlayBannerWrapper> l)
        {
            foreach (var i in l) i.DestroyBannerAd();
        }

        public static LevelPlayBannerWrapper ShowLargestRevenueBannerAd(this List<LevelPlayBannerWrapper> l)
        {
            LevelPlayBannerWrapper bo = null;
            foreach (var i in l.Where(i => i.CachedAdInfo != null))
            {
                if (bo == null)
                {
                    bo = i;
                    continue;
                }

                var br = bo.CachedAdInfo.Revenue ?? 0;
                var r = i.CachedAdInfo.Revenue ?? 0;
                if (br < r) bo = i;
            }
            if (bo == null) return null;
            bo.ShowBannerAd();
            bo.LogInfo("bo.Id", bo.Id, "revenue", l.ToRevenueMap());
            return bo;
        }
        
        private static Dictionary<string, string> ToRevenueMap(this List<LevelPlayBannerWrapper> l)
        {
            var revenue = new Dictionary<string, string>();
            foreach (var i in l)
            {
                var v = i.CachedAdInfo == null ? "null" : i.CachedAdInfo.FixedRevenue().ToString(CultureInfo.InvariantCulture);
                revenue.Add(i.Id,  v);
            }
            return revenue;
        }
        
        public static void OnBannerImpressionDataReadyEvent(this List<LevelPlayBannerWrapper> l, IronSourceImpressionData data)
        {
            foreach (var i in l) i.OnBannerImpressionDataReadyEvent(data);
        }
    }
}
#endif