#if BYTE_BREW
using ByteBrewSDK;
using Shared.Tracking.Templates;
using Shared.Utils;
using UnityEngine;

namespace Shared.Tracking.Internal.Handler.ByteBrewHandlers
{
    /// <summary>
    /// https://docs.bytebrew.io/sdk/unity#AdTrackingEvent
    /// </summary>
    public class ByteBrewCustomTrackingHandler : BaseTrackingHandler
    {
        public override void Handle(ITrackingEvent e)
        {
            if (e is IConvertableEvent ee && !Application.isEditor)
            {
                var p = ee.ToConvertableEvent().Convert();
                ByteBrew.NewCustomEvent(e.EventName, p);
            }      
        }
    }
}
#endif