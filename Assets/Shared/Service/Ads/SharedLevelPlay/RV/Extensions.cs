#if LEVEL_PLAY
using System.Collections.Generic;
using System.Globalization;
using Shared.Core.Resolver;
using Shared.Service.Ads.Common;
using UnityEngine;

namespace Shared.Service.Ads.SharedLevelPlay.RV
{
    public static class Extensions
    {
        public static string ResolveUserMessage(this IResolver<string, RewardedAdShowFailReason> resolver)
        {
            var isNotReachable = Application.internetReachability == NetworkReachability.NotReachable;
            return resolver.Resolve(isNotReachable ? RewardedAdShowFailReason.NoInternetConnection : RewardedAdShowFailReason.NotAvailable);
        }

        public static void DestroyAd(this List<LevelPlayRewardAdWrapper> l)
        {
            foreach (var i in l) i.DestroyAd();
        }
        
        public static void CreateAd(this List<LevelPlayRewardAdWrapper> l)
        {
            foreach (var i in l) i.Create();
        }
        
        public static Dictionary<string, string> ToRevenueMap(this List<LevelPlayRewardAdWrapper> l)
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