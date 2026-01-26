#if UNITY_IOS && IAP && ADJUST
using AdjustSdk;
using Shared.Core.IoC;
using Shared.Entity.Config;
using Shared.Service.Tracking.TrackingEvent.IAP;
using Shared.Tracking.Models.IAP;
using Shared.Tracking.Templates;
using Shared.Utils;
using Zenject;

namespace Shared.Service.Tracking.Handler.Adjust
{
    [Component]
    public class AppStoreAdjustTrackingHandler : ITrackingHandler, ISharedUtility
    {
        [Inject] private IConfig _config;
        
        public void Handle(ITrackingEvent e)
        {
            if (e is AppStoreInAppParams appStoreInAppParams)
            {
                // Iap
                var adjustEvent = new AdjustEvent(_config.AdjustRevenueEventToken)
                {
                    PurchaseToken = appStoreInAppParams.TransactionID
                };
                adjustEvent.SetRevenue((double)appStoreInAppParams.LocalizedPrice, appStoreInAppParams.IsoCurrencyCode);
                AdjustSdk.Adjust.TrackEvent(adjustEvent);
                this.LogInfo(SharedLogTag.TrackingNAdjustNIap, nameof(adjustEvent), adjustEvent);    
            } 
            else if (e is AppStoreSubscriptionParams appStoreSubscriptionParams) 
            {
                // Subscription
                AdjustSdk.Adjust.TrackAppStoreSubscription(new AdjustAppStoreSubscription(
                    price: appStoreSubscriptionParams.LocalizedPrice.ToLocalizePriceString(),
                    currency: appStoreSubscriptionParams.IsoCurrencyCode,
                    transactionId: appStoreSubscriptionParams.TransactionID));
            }
        }
        
    }
}
#endif