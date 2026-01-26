using Shared.Core.IoC;
using Shared.Core.Repository.Prefab;

namespace Shared.Core.View.Loading
{
    [Repository]
    public class UIActivityIndicatorPool : MonoBehaviourPool<IUIActivityIndicator>
    {
    }
}