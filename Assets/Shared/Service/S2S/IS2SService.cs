#if S2S
using Shared.Core.Async;
using Shared.Service.S2S.Request;

namespace Shared.Service.S2S
{
    public interface IS2SService
    {
        UserDataRequest GetUserData();
        IResultAsyncOperation<R> ExecutePost<R>(object o);
    }
}
#endif