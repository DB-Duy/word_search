using System.Collections.Generic;
using System.Linq;
using Shared.Core.Handler.Corou.Initializable;
using Shared.Utils;

namespace Shared.Core.IoC
{
    public static class InitializableRegistry
    {
        public static Dictionary<string, IInitializable> Items { get; } = new();

        public static void Register(string key, IInitializable item)
        {
            SharedLogger.LogInfoCustom(SharedLogTag.Ioc, nameof(InitializableRegistry), nameof(Register), nameof(key), key);
            Items.Add(key, item);
        }
        
        public static List<IInitializable> GetOrderedInitializables(string[] serviceOrders)
        {
            // Create the ordered list based on serviceOrders
            var orderedItems = serviceOrders
                .Where(order => Items.ContainsKey(order)) // Ensure the key exists in the dictionary
                .Select(order => Items[order])           // Fetch the corresponding IInitializable
                .ToList();

            // Append the remaining items that are not in serviceOrders
            var remainingItems = Items
                .Where(kvp => !serviceOrders.Contains(kvp.Key)) // Keys not in serviceOrders
                .Select(kvp => kvp.Value)                      // Fetch the corresponding IInitializable
                .ToList();

            // Concatenate the ordered items with the remaining items
            orderedItems.AddRange(remainingItems);

            return orderedItems;
        }
        
        
    }
}