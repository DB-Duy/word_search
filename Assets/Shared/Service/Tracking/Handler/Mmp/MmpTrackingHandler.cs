#if ADJUST || APPS_FLYER
using Shared.Core.IoC;
using Shared.Service.Mmp;
using Shared.Tracking.Templates;
using Zenject;

namespace Shared.Service.Tracking.Handler.Mmp
{
    /// <summary>
    /// https://ncalc.github.io/ncalc/articles/parameters.html
    /// </summary>
    [Component]
    public class MmpTrackingHandler : ITrackingHandler, ISharedUtility
    {
        [Inject] private MmpService _mmpService;

        public void Handle(ITrackingEvent e)
        {
            if (e is not IConvertableEvent ee) return;
            _mmpService.Track(e);
        }
    }
}
#endif