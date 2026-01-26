using Shared.Core.IoC;
using Shared.Repository.Tracking;
using Shared.Tracking.Templates;
using Zenject;

namespace Shared.Service.Tracking.EventModify
{
    [Component]
    public class TrackingEventCountModifier : ITrackingEventModifyHandler
    {
        [Inject] private IEventCountRepository _repository;

        public void Handle(ITrackingEvent e)
        {
            if (e is not ICountEvent ee) return;
            var count = _repository.IncreaseAndGet(ee.CountKey);
            ee.SetEventCount(count);
        }
    }
}