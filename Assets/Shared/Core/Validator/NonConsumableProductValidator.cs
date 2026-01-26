using System.Collections.Generic;
using System.Linq;
using Shared.Service.Iap;

namespace Shared.Core.Validator
{
    public class NonConsumableProductValidator : IValidator
    {
        // private const string Tag = "NonConsumableProductValidator";
        private readonly IIapService _iapService;
        private readonly HashSet<string> _nonConsumableProducts;

        public NonConsumableProductValidator(IIapService iapService, HashSet<string> nonConsumableProducts)
        {
            _iapService = iapService;
            _nonConsumableProducts = nonConsumableProducts;
        }
        
        public NonConsumableProductValidator(IIapService iapService, params string[] nonConsumableProducts)
        {
            _iapService = iapService;
            _nonConsumableProducts = new HashSet<string>(nonConsumableProducts);
        }

        public bool Validate()
        {
            return _nonConsumableProducts.Select(pid => _iapService.IsNonConsumablePackageOwned(pid)).All(result => !result);
        }
    }
}