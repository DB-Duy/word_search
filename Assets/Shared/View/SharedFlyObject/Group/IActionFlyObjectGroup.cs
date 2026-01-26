using System;

namespace Shared.View.SharedFlyObject.Group
{
    public interface IActionFlyObjectGroup : IFlyObjectGroup
    {
        void Fly(Action<IActionFlyObjectGroup> onComplete = null);
    }
}