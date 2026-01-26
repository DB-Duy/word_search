#if USING_UMP
using Shared.Core.Async;
using Shared.Utilities;
using Shared.Utils;

namespace Shared.Service.Ump.Internal.Steps
{
    public class SyncUmpExecutor : IStepExecutor, ISharedUtility, ISharedLogTag
    {   
        private IAsyncOperation _operation;
        public IAsyncOperation Execute(UmpProcessInfo i)
        {
            if (_operation != null) return _operation;
            _operation = new SharedAsyncOperation().Success();
            this.LogInfo();
            i.Service?.Sync();
            return _operation;
        }

        public string LogTag => SharedLogTag.Ump;
    }
}
#endif