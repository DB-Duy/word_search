using System.Collections.Generic;
using System.Linq;
using Shared.Core.IoC;
using Shared.Entity.Config;
using Shared.Tracking.Templates;
using Shared.Utils;
using Zenject;

namespace Shared.Service.Tracking.EventModify
{
    [Component]
    public class TrackingEventStaticPropertyModifier : ITrackingEventModifyHandler, ISharedUtility
    {
        [Inject] private IConfig _config;
        private string[] _propertyNames;
        private string[] PropertyNames => _propertyNames ??= _config?.TrackingEventStaticPropertyNames?.Split(",").ToArray();

        public void Handle(ITrackingEvent e)
        {
            if (e is not IExParamsEvent ee) return;
            if (PropertyNames == null || PropertyNames.Length == 0)
            {
                this.LogError(nameof(PropertyNames), "PropertyNames is null or empty");
                return;
            }
            List<string> missingProperties = new();
            foreach (var propertyName in PropertyNames)
            {
                var p = StaticEventPropertyRegistry.GetProperty(propertyName);
                if (p != null) ee.AddParams(propertyName, p.PropertyValue);
                else missingProperties.Add(propertyName);
            }

            if (missingProperties.Count > 0) this.LogError(nameof(missingProperties), missingProperties.ToJsonString());
        }
    }
}