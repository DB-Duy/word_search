#if IAP
using Shared.Core.Validator.Async;
using UnityEngine.Purchasing;

namespace Shared.Service.Iap.UnityCrossPlatform.Validator
{
    public interface IUnityIapReceiptValidator : IAsyncValidator<Product>
    {
    }
}
#endif