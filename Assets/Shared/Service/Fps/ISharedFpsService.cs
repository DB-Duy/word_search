using Shared.Service.Fps.Handlers;

namespace Shared.Service.Fps
{
    public interface ISharedFpsService
    {
        ISharedFpsService Add(params ISharedFpsHandler[] handlers);
    }
}