namespace Shared.Tracking.Property
{
    public interface ITrackingProperty
    {
        string PropertyName { get; }
        object PropertyValue { get; }
    }
}