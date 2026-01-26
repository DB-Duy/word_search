using System.Collections.Generic;
using System.Linq;
using Shared.Utils;
using Shared.View.InPlayAds;

namespace Shared.Service.InPlayAds
{
    public static class InPlayAdRegistry
    {
        /// <summary>
        /// Sử dụng để biết sẽ show ads của nhà cung cấp nào.
        /// </summary>
        public static InPlayAdProvider Provider { get; private set; }
        public static List<UIInPlayAdPlacementV2> Placements { get; } = new();
        public static List<UIInPlayAdMediator> Mediators { get; } = new();
        public static Dictionary<string, UIInPlayAdPlacement> V1Placements { get; } = new();

        public static void Register(UIInPlayAdPlacementV2 placement)
        {
            if (Placements.Contains(placement)) return;
            Placements.Add(placement);
        }
        
        public static void Remove(UIInPlayAdPlacementV2 placement)
        {
            if (!Placements.Contains(placement)) return;
            Placements.Remove(placement);
        }

        public static void RegisterPotentialProvider(InPlayAdProvider provider)
        {
            if (Provider != InPlayAdProvider.None) return;
            SharedLogger.LogInfoCustom(SharedLogTag.InPlayAds, nameof(InPlayAdRegistry), nameof(RegisterPotentialProvider), nameof(provider), provider.ToString());
            Provider = provider;
            
            foreach (var m in Mediators)
                m.OnPotentialProviderChanged(provider);
        }
        
        public static void ResetPotentialProvider()
        {
            SharedLogger.LogInfoCustom(SharedLogTag.InPlayAds, nameof(InPlayAdRegistry), nameof(ResetPotentialProvider));
            Provider = InPlayAdProvider.None;
        }
        
        public static void Register(UIInPlayAdMediator mediator)
        {
            if (Mediators.Contains(mediator)) return;
            Mediators.Add(mediator);
        }
        
        public static void Remove(UIInPlayAdMediator mediator)
        {
            if (!Mediators.Contains(mediator)) return;
            Mediators.Remove(mediator);
        }
        
        public static void Register(UIInPlayAdPlacement placement)
        {
            if (string.IsNullOrEmpty(placement.PlacementName)) return;
            V1Placements.TryAdd(placement.PlacementName, placement);
        }
        
        public static void Remove(UIInPlayAdPlacement placement)
        {
            if (string.IsNullOrEmpty(placement.PlacementName)) return;
            V1Placements.Remove(placement.PlacementName);
            foreach (var mediator in Mediators.Where(mediator => mediator.PlacementName == placement.PlacementName))
            {
                mediator.OnPlacementRemoved();
                break;
            }
        }

        public static UIInPlayAdPlacement GetPlacement(UIInPlayAdMediator mediator)
        {
            return V1Placements.GetValueOrDefault(mediator.PlacementName);
        }
    }
}