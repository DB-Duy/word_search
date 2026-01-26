using Shared.Service.Tracking.Common;
using Shared.Tracking.Templates;

namespace Shared.Service.Tracking
{
    public interface ITrackingService
    {
        void Track(TrackingScreen screen);
        void Track(ITrackingEvent e);
        
        void SetUserProperty(string key, object val);
    }
}