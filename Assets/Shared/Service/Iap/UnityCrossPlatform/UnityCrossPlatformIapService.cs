#if (APPSTORE || GOOGLE_PLAY) && IAP && !UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Shared.Core.Async;
using Shared.Core.IoC;
using Shared.Core.IoC.UnityLifeCycle;
using Shared.Entity.Iap;
using Shared.Repository.Iap;
using Shared.Service.Iap.Internal;
using Shared.Service.Iap.UnityCrossPlatform.Validator;
using Shared.Service.SharedCoroutine;
using Shared.Utils;
using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.Purchasing.Extension;
using Zenject;
using SharedPurchasing = Shared.Service.Iap.Internal.Purchasing;

namespace Shared.Service.Iap.UnityCrossPlatform
{
    [Service]
    public class UnityCrossPlatformIapService : IIapService, IUnityUpdate, IDetailedStoreListener, ISharedUtility
    {
        [Inject] private IIapItemRepository _itemRepository;
        [Inject(Optional = true)] private IUnityIapReceiptValidator _receiptValidator;
        [Inject(Optional = true)] private IIapDeliver _deliver;

        private IStoreController _storeController;
        private IExtensionProvider _storeExtensionProvider;

        public bool IsInitialized { get; private set; }
        private IAsyncOperation _initOperation;
        private IAsyncOperation<SharedPurchasing> _purchasingOperation;

        private readonly Queue<PurchaseEventArgs> _purchaseEventQueue = new();

// ---------------------------------------------------------------------------------------------------------------------
// Init
// ---------------------------------------------------------------------------------------------------------------------
        public IAsyncOperation Initialize()
        {
            if (_initOperation != null) return _initOperation;

            this.LogInfo(SharedLogTag.Iap);
            // Create a builder, first passing in a suite of Unity provided stores.
            var builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());

            // NonConsumable
            var nonConsumables = _itemRepository.GetNonConsumableItems();
            foreach (var i in nonConsumables)
            {
                this.LogInfo(SharedLogTag.Iap, nameof(i.ProductId), i.ProductId, "type", ProductType.NonConsumable.ToString());
                builder.AddProduct(i.ProductId, ProductType.NonConsumable);
            }
            // Consumable
            var consumables = _itemRepository.GetConsumableItems();
            foreach (var i in consumables)
            {
                this.LogInfo(SharedLogTag.Iap, nameof(i.ProductId), i.ProductId, "type", ProductType.Consumable.ToString());
                builder.AddProduct(i.ProductId, ProductType.Consumable);
            }

            // Subscriptions
            var subscriptions = _itemRepository.GetSubscriptionItems();
            foreach (var i in subscriptions)
            {
                this.LogInfo(SharedLogTag.Iap, nameof(i.ProductId), i.ProductId, "type", ProductType.Subscription.ToString());
                builder.AddProduct(i.ProductId, ProductType.Subscription);
            }

            _initOperation = new SharedAsyncOperation();
            UnityPurchasing.Initialize(this, builder);
            return _initOperation;
        }

        public void OnInitializeFailed(InitializationFailureReason error, string message)
        {
            this.LogInfo(SharedLogTag.Iap, nameof(error), error.ToString(), nameof(message), message);
            _initOperation.Fail($"{error.ToString()}-{message}");
        }

        public void OnInitializeFailed(InitializationFailureReason error)
        {
            this.LogInfo(SharedLogTag.Iap, nameof(error), error.ToString());
            _initOperation.Fail(error.ToString());
        }

        public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
        {
            this.LogInfo(SharedLogTag.Iap);
            // Purchasing has succeeded initializing. Collect our Purchasing references.
            // Overall Purchasing system, configured with products for this application.
            _storeController = controller;
            // Store specific subsystem, for accessing device-specific store features.
            _storeExtensionProvider = extensions;
            IsInitialized = true;
            _initOperation.Success();
#if LOG_INFO
            foreach (var p in controller.products.all)
            {
                this.LogInfo(SharedLogTag.Iap, "id", p.definition.id, "price", p.metadata.localizedPriceString, "name", p.metadata.localizedTitle, "hasReceipt", p.hasReceipt);
            }
#endif
        }
// ---------------------------------------------------------------------------------------------------------------------
// Purchase
// ---------------------------------------------------------------------------------------------------------------------
        public IAsyncOperation<SharedPurchasing> Purchase(string unknownId)
        {
            var productId = _ResolveProductId(unknownId);
            this.LogInfo(SharedLogTag.Iap, nameof(productId), productId, nameof(unknownId), unknownId);

            if (!IsInitialized)
            {
                this.LogError(SharedLogTag.Iap, nameof(productId), productId, nameof(IsInitialized), "false");
                return new SharedAsyncOperation<SharedPurchasing>(new SharedPurchasing(productId)).Fail("!IsInitialized()");    
            }

            if (_purchasingOperation != null)
            {
                if (_purchasingOperation.Data.ProductId == productId)
                {
                    return _purchasingOperation;
                }
                this.LogError(SharedLogTag.Iap, nameof(productId), productId, "reason", _purchasingOperation);
                return new SharedAsyncOperation<SharedPurchasing>(new SharedPurchasing(productId)).Fail("_purchasingOperation != null");
            }
            
            // ... look up the Product reference with the general product identifier and the Purchasing system's products collection.
            var product = _storeController.products.WithID(productId);
            if (product == null)
            {
                this.LogError(SharedLogTag.Iap, nameof(productId), productId, "reason", "product == null from _storeController.products.WithID(productId)");
                return new SharedAsyncOperation<SharedPurchasing>(new SharedPurchasing(productId)).Fail("product == null");
            }

            if (!product.availableToPurchase)
            {
                this.LogError(SharedLogTag.Iap, nameof(productId), productId, "reason", "!product.availableToPurchase");
                return new SharedAsyncOperation<SharedPurchasing>(new SharedPurchasing(productId)).Fail("!product.availableToPurchase");
            }

            // ... buy the product. Expect a response either through ProcessPurchase or OnPurchaseFailed asynchronously.
            _purchasingOperation = new SharedAsyncOperation<SharedPurchasing>(new SharedPurchasing(productId));
            _storeController.InitiatePurchase(product);
            return _purchasingOperation;
        }

        public void OnPurchaseFailed(Product product, PurchaseFailureDescription failureDescription)
        {
            // this.LogInfo(SharedLogTag.Iap, nameof(failureDescription.item.Product.definition.id), failureDescription.item.Product.definition.id, nameof(failureDescription.message), failureDescription.message, nameof(failureDescription.reason), failureDescription.reason.ToString());
            if (_purchasingOperation == null) return;
            if (product.definition.id != _purchasingOperation.Data.ProductId) return;

            _purchasingOperation.Fail($"{failureDescription.reason.ToString()} - {failureDescription.message}");
            _purchasingOperation = null;
        }

        public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason)
        {
            this.LogInfo(SharedLogTag.Iap, nameof(product.definition.id), product.definition.id, nameof(failureReason), failureReason);
            if (_purchasingOperation == null) return;
            if (product.definition.id != _purchasingOperation.Data.ProductId) return;

            _purchasingOperation.Fail($"{failureReason.ToString()}");
            _purchasingOperation = null;
        }

        public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs purchaseEvent)
        {
            this.LogInfo(SharedLogTag.Iap, "productId", purchaseEvent.purchasedProduct.definition.id, "receipt", purchaseEvent.purchasedProduct.receipt);
            _purchaseEventQueue.Enqueue(purchaseEvent);
            // Return a flag indicating whether this product has completely been received, or if the application needs 
            // to be reminded of this purchase at next app launch. Use PurchaseProcessingResult.Pending when still 
            // saving purchased products to the cloud, and when that save is delayed.
            return PurchaseProcessingResult.Pending;
        }

        public void Update()
        {
            if (Time.frameCount % 10 != 0) return;
            if (!IsInitialized) return;
            if (_purchaseEventQueue.Count <= 0) return;
            var e = _purchaseEventQueue.Dequeue();
            this.StartSharedCoroutine(_ProcessPurchasedProduct(e.purchasedProduct));
        }

        private IEnumerator _ProcessPurchasedProduct(Product purchasedProduct)
        {
            if (_receiptValidator != null)
            {
                var operation = _receiptValidator.Validate(purchasedProduct);
                if (operation == null)
                {
                    this.LogError("f", nameof(_ProcessPurchasedProduct), "error", "operation == null");
                    yield break;
                }

                while (!operation.IsComplete) yield return null;
                if (!operation.IsSuccess)
                {
                    this.LogError(SharedLogTag.Iap, "f", nameof(_ProcessPurchasedProduct), "reason", "InvalidReceipt", "productId", purchasedProduct.definition.id, "receipt", purchasedProduct.receipt);
                    _storeController.ConfirmPendingPurchase(purchasedProduct);
                    _purchasingOperation?.Fail("Receipt is invalid");
                    _purchasingOperation = null;
                    yield break;
                }
                this.LogInfo(SharedLogTag.Iap, "f", nameof(_ProcessPurchasedProduct), "validate.receipt.result", "success");
            }

            if (_deliver != null)
            {
                _deliver.Deliver(_storeController, purchasedProduct, _purchasingOperation);
            }
            else if (_deliver == null && _purchasingOperation != null)
            {
                this.LogInfo(SharedLogTag.Iap, "f", nameof(_ProcessPurchasedProduct), "operation", "Simulate _purchasingOperation by _deliver == null");
                _purchasingOperation.Success();
            }
            
            // Wait and clear flag of operation.
            if (_purchasingOperation == null) yield break;
            // Thêm _purchasingOperation != null để chắc chắn 1 điều khi đang loop check nếu bên ngoài có set nó bằng null thì cũng không sao. 
            while (_purchasingOperation != null && !_purchasingOperation.IsComplete) yield return null;
            _purchasingOperation = null;
        }
// ---------------------------------------------------------------------------------------------------------------------
// Process Pending Consumable Items.
// ---------------------------------------------------------------------------------------------------------------------
        public IAsyncOperation<SharedPurchasing> ProcessPendingConsumableByProductId(string unknownId)
        {
            var productId = _ResolveProductId(unknownId);
            if (!IsInitialized)
            {
                this.LogError(SharedLogTag.Iap, nameof(productId), productId, nameof(IsInitialized), "false");
                return new SharedAsyncOperation<SharedPurchasing>(new SharedPurchasing(productId)).Fail("!IsInitialized()");
            }
            if (_purchasingOperation != null)
            {
                if (_purchasingOperation.Data.ProductId != productId)
                {
                    this.LogError(SharedLogTag.Iap, nameof(productId), productId, "reason", "_purchasingOperation != null && _purchasingOperation.Data.ProductId != productId");
                    return new SharedAsyncOperation<SharedPurchasing>(new SharedPurchasing(productId)).Fail("_purchasingOperation != null && _purchasingOperation.Data.ProductId != productId");    
                }
                return _purchasingOperation;
            }

            var allProducts = _storeController.products.all;
            var p = allProducts.FirstOrDefault(p => p.definition.id == productId && p.definition.type == ProductType.Consumable && p.hasReceipt);
            if (p == null)
            {
                this.LogError(SharedLogTag.Iap, nameof(productId), productId, "reason", "p == null");
                return new SharedAsyncOperation<SharedPurchasing>(new SharedPurchasing(productId)).Fail("p == null");
            }
            _purchasingOperation = new SharedAsyncOperation<SharedPurchasing>(new SharedPurchasing(productId));
            this.StartSharedCoroutine(_ProcessPurchasedProduct(p));
            return _purchasingOperation;
        }

        public List<string> GetAllPendingConsumableProductId()
        {
            var allProducts = _storeController.products.all;
            return allProducts.Where(p => p.definition.type == ProductType.Consumable && p.hasReceipt).Select(p => p.definition.id).ToList();
        }
// ---------------------------------------------------------------------------------------------------------------------
// Subscriptions
// ---------------------------------------------------------------------------------------------------------------------
        public SharedSubscriptionInfo GetSubscriptionInfo(string unknownId)
        {
            var productId = _ResolveProductId(unknownId);
            var product = _storeController.products.WithID(productId);
            
            if (product == null)
            {
                this.LogInfo(SharedLogTag.Iap, "reason", "product == null", nameof(productId), productId);
                return null;
            }

            if (product.definition.type != ProductType.Subscription)
            {
                this.LogInfo(SharedLogTag.Iap, "reason", "product.definition.type != ProductType.Subscription", nameof(productId), productId, nameof(product.definition.type), product.definition.type);
                return null;
            }

            if (!product.hasReceipt)
            {
                // this.LogInfo("reason", "!product.hasReceipt", nameof(productId), productId, nameof(product.hasReceipt), product.hasReceipt);
                return null;
            }

            var p = new SubscriptionManager(product, null);
            var rawInfo = p.getSubscriptionInfo();
            return rawInfo?.ToSharedSubscriptionInfo();
        }
        
        public bool ValidateSubscription(string productId)
        {
            if (!IsInitialized) return false;
            var info = GetSubscriptionInfo(productId);
            // this.LogInfo(nameof(info), info);
            return info is { IsValidSubscription: true };
        }
// ---------------------------------------------------------------------------------------------------------------------
// Non-Consumables
// ---------------------------------------------------------------------------------------------------------------------
        public bool IsNonConsumablePackageOwned(string unknownId)
        {
            if (!IsInitialized)
            {
                this.LogError(SharedLogTag.Iap, "reason", "NotInitialized", nameof(unknownId), unknownId);
                return false;
            }
            var productId = _ResolveProductId(unknownId);
            var product = _storeController.products.WithID(productId);
            if (product == null)
            {
                this.LogError(SharedLogTag.Iap, "reason", "product == null", nameof(productId), productId);
                return false;
            }

            if (product.definition.type != ProductType.NonConsumable)
            {
                this.LogError(SharedLogTag.Iap, "reason", "product.definition.type != ProductType.NonConsumable", nameof(productId), productId, nameof(product.definition.type), product.definition.type);
                return false;
            }

            if (!product.hasReceipt)
            {
                this.LogError(SharedLogTag.Iap, "reason", "!product.hasReceipt", nameof(productId), productId, nameof(product.hasReceipt), product.hasReceipt);
                return false;
            }

            this.LogInfo(SharedLogTag.Iap, nameof(productId), productId);
            return true;
        }
// ---------------------------------------------------------------------------------------------------------------------
// Product Define
// ---------------------------------------------------------------------------------------------------------------------
        public SharedProductMetadata GetProduct(string unknownId)
        {
            var productId = _ResolveProductId(unknownId);
            if (!IsInitialized)
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
            
            var product = _storeController.products.WithID(productId);
            var metadata = product.metadata;
            return new SharedProductMetadata
            {
                LocalizedPriceString = metadata.localizedPriceString,
                LocalizedTitle = metadata.localizedTitle,
                LocalizedDescription = metadata.localizedDescription,
                IsoCurrencyCode = metadata.isoCurrencyCode,
                LocalizedPrice = metadata.localizedPrice
            };
        }

        public T GetReward<T>(string unknownId)
        {
            var productId = _ResolveProductId(unknownId);
            return _itemRepository.GetRewardByProductId<T>(productId);
        }
// ---------------------------------------------------------------------------------------------------------------------
// IAP APIs.
// ---------------------------------------------------------------------------------------------------------------------
        public void RestorePurchases()
        {
#if APPSTORE
            var appleExtensions = _storeExtensionProvider.GetExtension<IAppleExtensions>();
            appleExtensions.RestoreTransactions ((result, message) => {
                this.LogInfo(SharedLogTag.Iap, nameof(result), result, nameof(message), message);
                
            });
#else
            this.LogError("reason", $"Not supported on this platform. Current = {Application.platform.ToString()}");
#endif
        }

        public void RedirectingToSubscriptionManagementScreen(string unknownId = null)
        {
#if GOOGLE_PLAY
            var productId = _ResolveProductId(unknownId);
            var items = _itemRepository.GetSubscriptionItems();
            var dict = items.ToProductIdDict();
            var resolvedProductId = productId;
            if (string.IsNullOrEmpty(resolvedProductId) || !dict.ContainsKey(resolvedProductId))
            {
                resolvedProductId = items.Count > 0 ? items[0].ProductId : string.Empty;
            }
            // https://play.google.com/store/account/subscriptions?package=YOUR_PACKAGE_NAME&sku=YOUR_PRODUCT_ID
            var str = $"https://play.google.com/store/account/subscriptions?package={Application.identifier}&sku={resolvedProductId}";
            this.LogInfo(nameof(str), str);
            Application.OpenURL(str);
#elif APPSTORE
            Application.OpenURL("itms-apps://apps.apple.com/account/subscriptions");
#endif
        }

// ---------------------------------------------------------------------------------------------------------------------
// Common functions
// ---------------------------------------------------------------------------------------------------------------------
        private string _ResolveProductId(string unknownId)
        {
            return _itemRepository.ResolveProductId(unknownId);
        }
        
    }

    public static class ProductExtensions
    {
        public static Dictionary<string, object> ToDict(this Product product)
        {
            product.metadata.GetGoogleProductMetadata();
            return new Dictionary<string, object>()
            {
                {"definition", product.definition?.ToDict()},
                {"metadata", product.metadata?.ToDict()},
                {"availableToPurchase", product.availableToPurchase}
            };
        }

        public static Dictionary<string, object> ToDict(this ProductDefinition p)
        {
            return new Dictionary<string, object>()
            {
                {"id", p.id},
                {"enabled", p.enabled},
                {"type", p.type},
                {"storeSpecificId", p.storeSpecificId}
            };
        }

        public static Dictionary<string, object> ToDict(this ProductMetadata p)
        {
            return new Dictionary<string, object>()
            {
                {"localizedPriceString", p.localizedPriceString},
                {"localizedTitle", p.localizedTitle},
                {"localizedDescription", p.localizedDescription},
                {"isoCurrencyCode", p.isoCurrencyCode},
                {"localizedPrice", p.localizedPrice}
            };
        }
    }
}
#endif