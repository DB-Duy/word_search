#if IAP
using System.Collections.Generic;
using System.Linq;
using Shared.Utils;
using UnityEngine.Purchasing;

namespace Shared.Entity.Iap
{
    public static class IapItemExtensions
    {
        public static IapItem ToIapItem(this IapRawItem rawItem)
        {
#if GOOGLE_PLAY
            var productId = rawItem.GooglePlayProductId;
#elif APPSTORE
            var productId = rawItem.AppStoreProductId;
#endif
            return new IapItem()
            {
                Id = rawItem.Id,
                ProductId = CorrectProductId(productId),
                Type = rawItem.Type,
                DefaultUsdPrice = rawItem.DefaultUsdPrice.CastToFloat(),
                RewardJsonString = rawItem.RewardJsonString,
                LocalizationTitle = rawItem.LocalizationTitle
            };
        }
        
        public static Dictionary<string, IapItem> ToIapItem(this Dictionary<string, IapRawItem> rawItems)
            => rawItems.ToDictionary(i => i.Key, i => i.Value.ToIapItem());

        public static SharedProductMetadata ToProductMetadata(this ProductMetadata metadata)
        {
            return new SharedProductMetadata
            {
                LocalizedPriceString = metadata.localizedPriceString,
                LocalizedTitle = metadata.localizedTitle,
                LocalizedDescription = metadata.localizedDescription,
                IsoCurrencyCode = metadata.isoCurrencyCode,
                LocalizedPrice = metadata.localizedPrice
            };
        }
        
        public static SharedProductMetadata ToProductMetadata(this IapItem metadata)
        {
            return new SharedProductMetadata
            {
                LocalizedPriceString = $"${metadata.DefaultUsdPrice}",
                LocalizedTitle = metadata.LocalizationTitle,
                LocalizedDescription = metadata.LocalizationTitle,
                IsoCurrencyCode = "USD",// EUR
                LocalizedPrice = new decimal(metadata.DefaultUsdPrice)
            };
        }

        public static Dictionary<string, IapItem> ToIdDict(this List<IapItem> l) => l.ToDictionary(i => i.Id);
        public static Dictionary<string, IapItem> ToProductIdDict(this List<IapItem> l) => l.ToDictionary(i => i.ProductId);
        
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

        static string CorrectProductId(string productId)
        {
#if DEBUG_IAP
            return $"{productId}.debug" ;
#else
            return productId;
#endif
        }
    }
}
#endif