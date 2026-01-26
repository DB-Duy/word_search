using System.Collections.Generic;
using Newtonsoft.Json;
using Shared.Service.Tracking.Common;
using Shared.Tracking.Templates;

namespace Shared.Tracking.Models.IAP
{
    public class IapPurchasedParams : BaseTrackingEvent, IConvertableEvent
    {
        [JsonProperty("event_name")] public override string EventName => "iap_purchased";

        [JsonProperty("screen")] public IapScreen Screen { get; }
        [JsonProperty("package_id")] public IapPackageId PackageID { get; }
        
        [JsonIgnore] private Dictionary<string, object> _params;
        
        public IapPurchasedParams(IapPackageId iapPackageId, IapScreen screen)
        {
            Screen = screen;
            PackageID = iapPackageId;
        }

        public override string ToString() => JsonConvert.SerializeObject(ToConvertableEvent());
        
        public Dictionary<string, object> ToConvertableEvent()
        {
            return _params ??= new Dictionary<string, object>(ExParams)
            {
                { "screen", Screen.Value },
                { "package_id", PackageID.Value }
            };
        }
    }
}