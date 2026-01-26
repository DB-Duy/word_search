using System.Collections.Generic;
using Shared.Core.IoC;
using Shared.Core.Repository.IntType;
using Shared.Service.Tracking.Common;
using Shared.Tracking.Models.Game;
using Shared.Tracking.Property;
using Shared.Tracking.Templates;
using Shared.Tracking.Utils;

namespace Shared.Service.Tracking
{
    public static class TrackingUtility
    {
        public static ITrackingService TrackingService { get; set; }

        public static void Track(this ISharedUtility o, ITrackingEvent e) => TrackingService?.Track(e);
        public static void Track(this ISharedUtility o, TrackingScreen screen) => TrackingService?.Track(screen);

        public static void SetTrackingProperty(this ISharedUtility o, ITrackingProperty property) => StaticEventPropertyRegistry.SetProperties(property);
        public static void SetTrackingProperty(this ISharedUtility o, string key, object val) => StaticEventPropertyRegistry.SetProperties(key, val);
        public static void SetTrackingProperties(this ISharedUtility o, params ITrackingProperty[] properties) => StaticEventPropertyRegistry.SetProperties(properties);
        public static void SetTrackingProperties(this ISharedUtility o, params object[] keyValueParams) => StaticEventPropertyRegistry.SetProperties(keyValueParams);
        public static void SetTrackingProperties(this ISharedUtility o, Dictionary<string, object> properties) => StaticEventPropertyRegistry.SetProperties(properties);
        public static void SetUserProperty(this ISharedUtility o, string key, object v) => TrackingService?.SetUserProperty(key, v);
        public static ITrackingProperty GetTrackingProperty(this ISharedUtility o, string name) => StaticEventPropertyRegistry.GetProperty(name);

        public static void RemoveTrackingProperties(this ISharedUtility o, params string[] propertyNames) => StaticEventPropertyRegistry.RemoveProperties(propertyNames);
        
        public static void AddMoreAndTrackEarn(this IIntRepository r, int more, CurrencyName currencyName, EventSource source)
        {
            r.AddMore(more);
            var earnEvent = GameResourceParams.Earned(
                currencyAmount: more,
                currencyName: currencyName,
                source: source
            );
            TrackingService?.Track(earnEvent);
        }

        public static void MinusAndTrackSpent(this IIntRepository r, int less, CurrencyName currencyName, EventSource source)
        {
            r.Minus(less);
            var spentEvent = GameResourceParams.Spent(
                currencyAmount: less,
                currencyName: currencyName,
                source: source
            );
            TrackingService?.Track(spentEvent);
        }

    }
}