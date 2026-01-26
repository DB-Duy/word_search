using System;
using System.Collections.Generic;
using Shared.Service.Tracking.Property;
using Shared.Tracking.Property;
using Shared.Utils;

namespace Shared.Service.Tracking
{
    /// <summary>
    /// Store all properties that used in the project.
    /// </summary>
    public static class StaticEventPropertyRegistry
    {
        private const string ClassName = "StaticEventPropertyRegistry";
        
        private static readonly Dictionary<string, ITrackingProperty> Properties = new();
        
        public static void SetProperty(string key, object val) 
            => SetProperty(new TrackingProperty(key, val));

        public static void SetProperties(params ITrackingProperty[] properties)
        {
            foreach (var p in properties) SetProperty(p);
        }

        public static void SetProperties(params object[] keyValueParams)
        {
            if (keyValueParams.Length % 2 != 0)
                throw new Exception($"Invalid keyValueParams with length {keyValueParams.Length}");
            for (var i = 0; i < keyValueParams.Length - 1; i += 2)
                if (keyValueParams[i] is not string)
                    throw new Exception($"keyValueParams[{i}] is not string type.");
            for (var i = 0; i < keyValueParams.Length - 1; i += 2)
                SetProperty(new TrackingProperty(keyValueParams[i].ToString(), keyValueParams[i + 1]));
        }

        public static void SetProperties(Dictionary<string, object> properties)
        {
            foreach (var e in properties)
            {
                if (string.IsNullOrEmpty(e.Key)) throw new Exception("string.IsNullOrEmpty(e.Key)");
                if (e.Value == null) throw new Exception($"e.Value == null for {e.Key}");
            }

            foreach (var e in properties) SetProperty(new TrackingProperty(e.Key, e.Value));
        }

        public static void SetProperty(ITrackingProperty property)
        {
            SharedLogger.LogInfoCustom(SharedLogTag.Tracking, ClassName, nameof(SetProperty), nameof(property), property);
            Properties[property.PropertyName] = property;
        }

        public static ITrackingProperty GetProperty(string name) => Properties.ContainsKey(name) ? Properties[name] : null;

        public static void RemoveProperties(params string[] propertyNames)
        {
            foreach (var n in propertyNames) Properties.Remove(n);
        }
    }
}