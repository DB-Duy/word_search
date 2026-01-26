using Shared.Utilities.SharedBehaviour;

namespace Shared.Core.IoC
{
    public class IoCMonoBehavior : SharedMonoBehaviour, IIoC
    {
        protected virtual void Awake()
        {
            this.InjectDependencies();
        }
    }
}