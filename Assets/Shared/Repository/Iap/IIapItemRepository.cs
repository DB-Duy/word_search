using System.Collections.Generic;
using Shared.Entity.Iap;

namespace Shared.Repository.Iap
{
    public interface IIapItemRepository
    {
        Dictionary<string, IapItem> GetAll();
        
        IapItem GetById(string id);
        IapItem GetByProductId(string productId);

        List<IapItem> GetConsumableItems();
        List<IapItem> GetSubscriptionItems();
        List<IapItem> GetNonConsumableItems();
        List<IapItem> GetItemsByType(string type);
        
        T GetRewardById<T>(string id);
        T GetRewardByProductId<T>(string productId);

        T ParseReward<T>(string rewardJson);

        bool IsSubscriptionProductId(string productId);
        bool IsConsumableProductId(string productId);
        bool IsNonConsumableProductId(string productId);

        bool IsItemId(string id);
        string ResolveProductId(string unknownId);
    }
}