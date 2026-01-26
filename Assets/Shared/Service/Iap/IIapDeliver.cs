#if IAP
using Shared.Core.Async;
using SharedPurchasing = Shared.Service.Iap.Internal.Purchasing;

namespace Shared.Service.Iap
{
    public interface IIapDeliver
    {
        void Deliver(UnityEngine.Purchasing.IStoreController storeController,  UnityEngine.Purchasing.Product purchasedProduct, IAsyncOperation<SharedPurchasing> operation);
    }
}
#endif