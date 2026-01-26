using Shared.Core.IoC;
using Shared.Repository.Iap;

namespace Shared.Service.Iap
{
    public static class IapExtensions
    {
        public static string ToIapProductId(this string itemId)
        {
            var r = IoCExtensions.Resolve<IIapItemRepository>();
            return r.IsItemId(itemId) ? r.GetById(itemId).ProductId : itemId;
        }
    }
}