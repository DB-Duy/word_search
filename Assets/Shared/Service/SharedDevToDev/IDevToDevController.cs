using Shared.Core.Handler.Corou.Initializable;
using Shared.SharedDevToDev.Internal;

namespace Shared.SharedDevToDev
{
    public interface IDevToDevController : IInitializable
    {
        IDevToDevController Setup(IDevToDevConfig config);
    }
}