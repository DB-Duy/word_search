#if USING_UMP
using Shared.Core.Async;

namespace Shared.Service.Ump.Internal
{
    public interface IStepExecutor
    {
        IAsyncOperation Execute(UmpProcessInfo i);
    }
}
#endif