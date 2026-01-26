namespace Shared.Tracking.Templates
{
    public interface ICountEvent
    {
        string CountKey { get; }
        void SetEventCount(long count);
    }
}