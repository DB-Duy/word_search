#if BYTE_BREW && GOOGLE_PLAY
using ByteBrewSDK;
using Shared.Tracking.Models.IAP;
using Shared.Tracking.Templates;

namespace Shared.Tracking.Internal.Handler.ByteBrewHandlers
{
    /// <summary>
    /// https://docs.bytebrew.io/sdk/unity#AdTrackingEvent
    /// </summary>
    public class ByteBrewGooglePlaySubscriptionTrackingHandler : BaseTrackingHandler
    {
        public override void Handle(ITrackingEvent e)
        {
            if (e is GooglePlaySubscriptionParams sub)
            {
                ByteBrew.TrackGoogleInAppPurchaseEvent("GooglePlay", "USD", sub.DefaultUsdPriceValue, sub.ProductId, "DefaultCategory", sub.PurchaseToken, sub.Signature);
            }   
        }
    }
}
#endif