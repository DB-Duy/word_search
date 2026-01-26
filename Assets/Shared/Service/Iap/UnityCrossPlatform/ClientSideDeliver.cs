#if IAP_CLIENT_DELIVER
using Shared.Core.Async;
using Shared.Core.Handler;
using Shared.Core.IoC;
using Shared.Entity.Iap;
using Shared.Repository.Iap;
using Shared.Service.Iap.Internal;
using Shared.Service.Loading.Handlers;
using Shared.Service.Tracking;
using Shared.Service.Tracking.TrackingEvent.IAP;
using Shared.Utils;
using UnityEngine.Purchasing;
using Zenject;
using SharedPurchasing = Shared.Service.Iap.Internal.Purchasing;

namespace Shared.Service.Iap.UnityCrossPlatform
{
    [Component]
    public class ClientSideDeliver : IIapDeliver, ISharedUtility
    {
        [Inject(Optional = true)] private IIapItemRepository _iapItemRepository;
        private IHandler<string> _cachedSubscriptionHandler;
        private IHandler<string> SubscriptionHandler => _cachedSubscriptionHandler ??= SequenceHandlerChain<string>.CreateChainFromType<ISubscriptionHandler>();
        
        private IHandler<string> _cachedNonConsumableHandler;
        private IHandler<string> NonConsumableHandler => _cachedNonConsumableHandler ??= SequenceHandlerChain<string>.CreateChainFromType<INonConsumableHandler>();
        
        private IHandler _cachedPostInitHandler;
        public IHandler PostInitHandler => _cachedPostInitHandler ??= SequenceHandlerChain.CreateChainFromType<IPostInitHandler>();
        
        private IHandler<string> _consumableSilentHandler;
        public IHandler<string> ConsumableSilentHandler => _consumableSilentHandler ??= HandlerDictionary.CreateDictionaryFromType<IConsumableSilentHandler>();
        
        public void Deliver(IStoreController storeController, Product purchasedProduct, IAsyncOperation<SharedPurchasing> operation)
        {
            this.LogInfo(SharedLogTag.Iap, nameof(operation), operation == null ? "null" : "NOT null", "id", purchasedProduct.definition.id);
            // 1. Confirm PurchaseEventArgs first.
            storeController.ConfirmPendingPurchase(purchasedProduct);
            // 2. Tracking PurchaseEventArgs.
            _TrackPurchaseEventArgs(purchasedProduct);
            // 3. Deliver 
            _DeliverByPurchaseEventArgs(purchasedProduct, operation);
        }
        
        private void _TrackPurchaseEventArgs(Product purchasedProduct)
        {
            switch (purchasedProduct.definition.type)
            {
                case ProductType.NonConsumable:
                case ProductType.Consumable:
                {
#if APPSTORE
                    var defineItem = _iapItemRepository.GetByProductId(purchasedProduct.definition.id);
                    this.Track(new AppStoreInAppParams(purchasedProduct, defineItem.DefaultUsdPrice));
#elif GOOGLE_PLAY 
                    var defineItem = _iapItemRepository.GetByProductId(purchasedProduct.definition.id);
                    var receipt = GooglePlayInAppReceipt.NewInstance(purchasedProduct.receipt);
                    this.Track(GooglePlayInAppParams.Of(purchasedProduct, receipt, defineItem.DefaultUsdPrice));
#endif
                    break;
                }
                case ProductType.Subscription:
                {
#if APPSTORE
                    var defineItem = _iapItemRepository.GetByProductId(purchasedProduct.definition.id);
                    this.Track(new AppStoreSubscriptionParams(purchasedProduct, defineItem.DefaultUsdPrice));
#elif GOOGLE_PLAY 
                    var defineItem = _iapItemRepository.GetByProductId(purchasedProduct.definition.id);
                    var receipt = GooglePlaySubscriptionReceipt.NewInstance(purchasedProduct.receipt);
                    this.Track(GooglePlaySubscriptionParams.Of(purchasedProduct, receipt, defineItem.DefaultUsdPrice));
#endif
                    break;
                }
            }
        }
        
        private void _DeliverByPurchaseEventArgs(Product purchasedProduct, IAsyncOperation<SharedPurchasing> operation)
        {
            var productId = purchasedProduct.definition.id;
            if (purchasedProduct.definition.type == ProductType.NonConsumable)
            {
                this.LogInfo(SharedLogTag.Iap, nameof(productId), productId, "call", $"NonConsumableHandler.Handle({productId});");
                NonConsumableHandler.Handle(productId);
            } 
            else if (purchasedProduct.definition.type == ProductType.Consumable)
            {
                if (operation == null || operation.Data.ProductId != productId)
                {
                    this.LogInfo(SharedLogTag.Iap, nameof(productId), productId, "call", $"ConsumableSilentHandler.Handle({productId});");
                    ConsumableSilentHandler?.Handle(productId);
                }
            } 
            else if (purchasedProduct.definition.type == ProductType.Subscription)
            {
                SubscriptionHandler?.Handle(productId);
            }

            if (operation == null || operation.Data.ProductId != productId) return;
            operation.Success();
        }
    }
}
#endif