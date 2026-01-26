using Shared.Core.Async;

namespace Shared.View.SharedFlyObject
{
    public interface IAsyncFlyObject : ISharedFlyObject
    {
        IAsyncOperation Fly();
    }
}