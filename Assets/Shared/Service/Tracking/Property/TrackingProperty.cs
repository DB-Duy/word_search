using Newtonsoft.Json;
using Shared.Service.Tracking.Common;
using Shared.Tracking.Property;

namespace Shared.Service.Tracking.Property
{
    [System.Serializable]
    public class TrackingProperty : ITrackingProperty
    {
        [JsonProperty("n")] public string PropertyName { get; }
        [JsonProperty("v")] public object PropertyValue { get; }

        public TrackingProperty(string propertyName, object propertyValue)
        {
            PropertyName = propertyName;
            PropertyValue = propertyValue switch
            {
                ValueObject valueObject => valueObject.Value,
                TrackingProperty trackingProperty => trackingProperty.PropertyValue,
                _ => propertyValue
            };
#if LOG_INFO
            switch (PropertyValue)
            {
                case string or int or long or float or double or bool:
                    break;
                default:
                    throw new System.Exception($"Unknown property value type for {PropertyValue}: {PropertyValue.GetType().FullName}");
            }
#endif
        }
        
        public override string ToString() => JsonConvert.SerializeObject(this);

        public static TrackingProperty Level(int value) => new(PropertyConst.LEVEL, value);
        public static TrackingProperty GameMode(GameMode value) => new(PropertyConst.GAME_MODE, value);
        public static TrackingProperty LevelDesign(string levelDesign) => new(PropertyConst.LEVEL_DESIGN, levelDesign);
    }
}