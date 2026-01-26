#if HUAWEI
using HuaweiMobileServices.IAP;
using Shared.IAP.Common;

namespace Shared.IAP.HuaweiIap
{
    public class HuaweiIapProduct : IIapProduct
    {
        public string LocalizedPriceString => _productInfo.Price;

        private readonly ProductInfo _productInfo;

        public HuaweiIapProduct(ProductInfo productInfo)
        {
            _productInfo = productInfo;
        }
    }
}
#endif