using System.Collections.Generic;
using Newtonsoft.Json;
using Shared.Tracking.Templates;

namespace Shared.Service.Tracking.TrackingEvent.Ads
{
    public class AdImpressionEvent : BaseTrackingEvent, IConvertableEvent
    {
        public override string EventName => "ad_impression";

        [JsonProperty("ad_platform")] public string AdPlatform { get; }
        [JsonProperty("ad_source")] public string AdSource { get; }
        [JsonProperty("ad_unit_name")] public string AdUnitName { get; }
        [JsonProperty("ad_format")] public string AdFormat { get; }
        [JsonProperty("currency")] public string Currency { get; }
        [JsonProperty("value")] public double Value { get; }
        [JsonProperty("country")] public string Country { get; set; }
        [JsonProperty("ad_unit_id")] public string AdUnitId { get; set; }
        [JsonProperty("placement")] public string Placement { get; set; }
        
        [JsonProperty("auction_id")] public string AuctionId { get; set; }
        [JsonProperty("creative_id")] public string CreativeId { get; set; }
        [JsonProperty("mediation_ad_unit_name")] public string MediationAdUnitName { get; set; }
        [JsonProperty("mediation_ad_unit_id")] public string MediationAdUnitId { get; set; }
        [JsonProperty("ab")] public string Ab { get; set; }
        [JsonProperty("segment_name")] public string SegmentName { get; set; }
        [JsonProperty("instance_name")] public string InstanceName { get; set; }
        [JsonProperty("instance_id")] public string InstanceId { get; set; }
        [JsonProperty("precision")] public string Precision { get; set; }
        [JsonProperty("encrypted_cpm")] public string EncryptedCpm { get; set; }
        [JsonProperty("conversion_value")] public string ConversionValue { get; set; }
        
        
        [JsonIgnore] private Dictionary<string, object> _params;

        public AdImpressionEvent(string adPlatform, string adSource, string adUnitName, string adFormat, string currency, double value)
        {
            AdPlatform = adPlatform;
            AdSource = adSource;
            AdUnitName = adUnitName;
            AdFormat = adFormat;
            Currency = currency;
            Value = value;
        }

        public override string ToString() => JsonConvert.SerializeObject(ToConvertableEvent());
        
        public Dictionary<string, object> ToConvertableEvent()
        {
            return _params ??= new Dictionary<string, object> (ExParams)
            {
                { "ad_platform", AdPlatform },
                { "ad_source", AdSource },
                { "ad_unit_name", AdUnitName },
                { "ad_format", AdFormat },
                { "currency", Currency },
                { "value", Value },
                { "mediation_ad_unit_name", MediationAdUnitName ?? "null" },
                { "mediation_ad_unit_id", MediationAdUnitId ?? "null" },
            };
        }
    }
}