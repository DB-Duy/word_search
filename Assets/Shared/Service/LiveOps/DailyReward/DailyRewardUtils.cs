using System;

namespace Shared.LiveOps.DailyReward
{
    public class DailyRewardUtils
    {
        public static string GetTodayId()
        {
            return DateTime.Now.ToString("yyyy-MM-dd");
        }
    }
}