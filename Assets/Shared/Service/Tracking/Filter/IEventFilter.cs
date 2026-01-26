using Shared.Tracking.Templates;

namespace Shared.Service.Tracking.Filter
{
    public interface IEventFilter
    {
        bool Validate(ITrackingEvent e);
    }
}