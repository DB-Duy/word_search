#if HUAWEI
using System.Collections.Generic;
using HuaweiMobileServices.IAP;

namespace Shared.IAP.HuaweiIap
{
    public class HuaweiCachedLastSubscriptionStateDict
    {
        private readonly Dictionary<string, HuaweiCachedLastSubscriptionState> _stateDict = new();
        private readonly IHuaweiIapController _huaweiIapController;

        public bool Enable { get; set; }

        public HuaweiCachedLastSubscriptionStateDict(IHuaweiIapController huaweiIapController)
        {
            _huaweiIapController = huaweiIapController;
        }


        public void Add(InAppPurchaseData entity)
        {
            var productId = entity.ProductId;
            var isValid = _huaweiIapController.ValidateSubscription(productId);
            if (!_stateDict.ContainsKey(productId)) _stateDict.Add(productId, new HuaweiCachedLastSubscriptionState(productId, isValid));
        }

        public void Add(bool clearAllFlag, params InAppPurchaseData[] arrayOfData)
        {
            if (clearAllFlag) ClearAll();
            foreach (var entity in arrayOfData) Add(entity);
        }

        public void ClearAll() => _stateDict.Delete();

        public bool ValidateSubscription(string productId) => _stateDict.ContainsKey(productId) && _stateDict[productId].IsValid;
    }
}
#endif