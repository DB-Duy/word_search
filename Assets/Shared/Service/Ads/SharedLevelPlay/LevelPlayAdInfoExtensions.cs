#if LEVEL_PLAY
using Unity.Services.LevelPlay;

namespace Shared.Service.Ads.SharedLevelPlay
{
    public static class LevelPlayAdInfoExtensions
    {
        public static string DebugLessField(this LevelPlayAdInfo adInfo)
        {
            return adInfo == null ? "null" : StringUtils.ToJsonString("InstanceId", adInfo.InstanceId, "AdNetwork", adInfo.AdNetwork, "Revenue", adInfo.Revenue, "adSize", adInfo.AdSize);
        }
        
        public static double FixedRevenue(this LevelPlayAdInfo adInfo) => adInfo?.Revenue ?? 0;
    }
}
#endif