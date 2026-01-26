using Shared.Core.IoC;
using Shared.Core.Repository.Prefab;

namespace Shared.Core.View.FloatingMessage
{
    [Repository]
    public class UIFloatingMessagePool : MonoBehaviourPool<IUIFloatingMessage>
    {
    }
}