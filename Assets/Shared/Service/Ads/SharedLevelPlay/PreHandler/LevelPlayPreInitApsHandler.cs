#if (USE_APS || APS) && LEVEL_PLAY
using System.Collections;
using AmazonAds;
using Shared.Core.IoC;
using Shared.Entity.Config;
using Shared.Utils;
using Zenject;

namespace Shared.Service.Ads.SharedLevelPlay.PreHandler
{
    [Component]
    public class LevelPlayPreInitApsHandler : ILevelPlayPreInitHandler, ISharedUtility
    {
        [Inject] private IConfig _config;
        
        public IEnumerator Handle()
        {
            var apsAppId = _config.ApsAppId;
            this.LogInfo(SharedLogTag.AdNLevelPlayNAps, nameof(apsAppId), apsAppId);
            Amazon.Initialize(apsAppId);
            //Amazon.SetAdNetworkInfo(new AdNetworkInfo(DTBAdNetwork.IRON_SOURCE));
#if LOG_INFO
            Amazon.EnableTesting(true);
#endif
#if LOG_INFO && UNITY_ANDROID
            // Do IOS bị crash khi bật log, nên chỉ enable log cho Android.
            Amazon.EnableLogging(true);
#endif
            yield break;
        }
    }
}

#endif