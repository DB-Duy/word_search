namespace Shared.Core.Async
{
    public interface IScreenTrackingHandler
    {
        void OnScreenTrackingEvent(string newScreenName);
    }
}