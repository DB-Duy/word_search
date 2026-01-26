#if IAP
#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using Shared.Core.Async;
using Shared.Core.Handler;
using Shared.Core.IoC;
using Shared.Entity.Iap;
using Shared.Repository.Iap;
using Zenject;
using SharedPurchasing = Shared.Service.Iap.Internal.Purchasing;


namespace Shared.Service.Iap
{
    [Service]
    public class FakeIapService : IIapService
    {
        [Inject] private IapItemRepository _itemRepository;
        
        private IAsyncOperation _initOperation;

        public bool IsInitialized { get; private set; }

        public IAsyncOperation Initialize()
        {
            if (_initOperation != null) return _initOperation;
            IsInitialized = true;
            _initOperation = new SharedAsyncOperation().Success();
            return _initOperation;
        }

        public IAsyncOperation<SharedPurchasing> Purchase(string productId)
        {
            return new SharedAsyncOperation<SharedPurchasing>(new SharedPurchasing(productId)).Success();
        }

        public IAsyncOperation<SharedPurchasing> ProcessPendingConsumableByProductId(string productId)
        {
            throw new NotImplementedException();
        }

        public List<string> GetAllPendingConsumableProductId()
        {
            throw new NotImplementedException();
        }

        public void ProcessUnhandledConsumable(string productId, IHandler handler)
        {
            handler.Handle();
        }

        public bool ValidateSubscription(string productId)
        {
            return false;
        }

        public bool IsNonConsumablePackageOwned(string productId)
        {
            return false;
        }

        public SharedProductMetadata GetProduct(string productId)
        {
            var i = _itemRepository.GetByProductId(productId);
            return new SharedProductMetadata
            {
                LocalizedPriceString = $"${i.DefaultUsdPrice}",
                LocalizedTitle = i.LocalizationTitle,
                LocalizedDescription = i.LocalizationTitle,
                IsoCurrencyCode = "USD",// EUR
                LocalizedPrice = new decimal(i.DefaultUsdPrice)
            };
        }

        public T GetReward<T>(string productId) => _itemRepository.GetRewardByProductId<T>(productId);

        public SharedSubscriptionInfo GetSubscriptionInfo(string productId)
        {
            return new SharedSubscriptionInfo()
            {
                IsSubscribed = false,
                IsExpired = false,
                IsFreeTrial = false,
                IsCancelled = false
            };
        }

        public void RestorePurchases()
        {
            throw new NotImplementedException();
        }

        public void RedirectingToSubscriptionManagementScreen(string productId = null)
        {
            throw new NotImplementedException();
        }
    }
}
#endif
#endif