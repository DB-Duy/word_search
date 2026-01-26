#if IAP
using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Shared.Core.IoC;
using Shared.Core.Repository.ResourceJsonType;
using Shared.Entity.Iap;

namespace Shared.Repository.Iap
{
    [Repository]
    public class IapItemRepository : IIapItemRepository
    {
        private const string Tag = "IapItemRepository";

        private Dictionary<string, IapItem> _iapItems;
        
        public Dictionary<string, IapItem> GetAll()
        {
            if (_iapItems != null) return _iapItems;
            var repo = new JsonResourceRepository<Dictionary<string, IapRawItem>>("Iap/IapItem");
            var rawItems = repo.Get();
            _iapItems = new Dictionary<string, IapItem>();
            foreach (var rt in rawItems)
                _iapItems.Add(rt.Value.Id, IapItem.Of(rt.Value));
            return _iapItems;
        }

        public IapItem GetById(string id)
        {
            var dict = GetAll();
            return dict.TryGetValue(id, out var value) ? value : null;
        }

        public IapItem GetByProductId(string productId)
        {
            var dict = GetAll();
            return (from e in dict where e.Value.ProductId == productId select e.Value).FirstOrDefault();
        }

        public List<IapItem> GetConsumableItems() => GetItemsByType(IapItemType.KConsumable);

        public List<IapItem> GetSubscriptionItems() => GetItemsByType(IapItemType.KSubscription);

        public List<IapItem> GetNonConsumableItems() => GetItemsByType(IapItemType.KNonConsumable);

        public List<IapItem> GetItemsByType(string type)
        {
            var dict = GetAll();
            return (from e in dict where e.Value.Type == type select e.Value).ToList();
        }

        public T GetRewardById<T>(string id)
        {
            var i = GetById(id);
            return ParseReward<T>(i.RewardJsonString);
        }

        public T GetRewardByProductId<T>(string productId)
        {
            var i = GetByProductId(productId);
            return ParseReward<T>(i.RewardJsonString);
        }

        public T ParseReward<T>(string rewardJson)
        {
            if (string.IsNullOrEmpty(rewardJson)) 
                return default;
            if (!rewardJson.IsValidJson())
                throw new Exception($"[{SharedLogTag.Iap}] {Tag}->ParseReward. !rewardJson.IsValidJson() {rewardJson}");
            return JsonConvert.DeserializeObject<T>(rewardJson);
        }

        public bool IsSubscriptionProductId(string productId)
        {
            var i = GetByProductId(productId);
            return i.Type == IapItemType.KSubscription;
        }

        public bool IsConsumableProductId(string productId)
        {
            var i = GetByProductId(productId);
            return i.Type == IapItemType.KConsumable;
        }

        public bool IsNonConsumableProductId(string productId)
        {
            var i = GetByProductId(productId);
            return i.Type == IapItemType.KNonConsumable;
        }

        public bool IsItemId(string id) => GetAll().ContainsKey(id);
        
        public string ResolveProductId(string unknownId)
        {
            var productDict = GetAll();
            return productDict.TryGetValue(unknownId, out var value) ? value.ProductId : unknownId;
        }
    }
}
#endif