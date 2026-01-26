using Shared.Core.Async;

namespace Shared.Core.Handler.Corou.Initializable
{
    public interface IInitializable
    {
        bool IsInitialized { get; }
        IAsyncOperation Initialize();
    }
}