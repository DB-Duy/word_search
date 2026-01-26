using System;

namespace Shared.View.SharedFlyObject
{
    public interface IActionFlyObject : ISharedFlyObject
    {
        void Fly(Action<ISharedFlyObject> onComplete = null);
    }
}