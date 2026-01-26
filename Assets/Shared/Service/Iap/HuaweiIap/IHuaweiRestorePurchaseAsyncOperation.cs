using Shared.Core.Async;

namespace Shared.IAP.HuaweiIap
{
    public interface IHuaweiRestorePurchaseAsyncOperation : IAsyncOperation
    {
        void RestorePurchaseRecordsComplete();
        void RestorePurchaseRecordsFailure();
        
        void RestoreOwnedPurchasesComplete();
        void RestoreOwnedPurchasesFailure();
    }
}