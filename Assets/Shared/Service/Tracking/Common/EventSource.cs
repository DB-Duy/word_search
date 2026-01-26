namespace Shared.Service.Tracking.Common
{
    public class EventSource : ValueObject
    {
        public EventSource(string v) : base(v)
        {
        }

        public static readonly EventSource Unknown = new("unknown");
    }
}