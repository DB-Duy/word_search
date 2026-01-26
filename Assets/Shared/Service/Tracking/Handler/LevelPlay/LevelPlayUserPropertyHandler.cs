#if LEVEL_PLAY
using System.Collections.Generic;
using Shared.Core.Handler;
using Shared.Core.IoC;
using Shared.Utils;

namespace Shared.Service.Tracking.Handler.LevelPlay
{
    [Component]
    public class LevelPlayUserPropertyHandler : IHandler<string, object>, ISharedUtility, IUserPropertyHandler
    {
        public void Handle(string k, object v)
        {
            var fv = v.ToString();
            this.LogInfo(SharedLogTag.UserPropertyNLevelPlay, nameof(k), k, nameof(fv), fv);
            var segment = new IronSourceSegment
            {
                customs = new Dictionary<string,string> { { k, fv } }
            };
            IronSource.Agent.setSegment(segment);
        }
    }
}
#endif