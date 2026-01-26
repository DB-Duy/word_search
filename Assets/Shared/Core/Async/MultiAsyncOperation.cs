using System.Collections.Generic;
using System.Linq;
using Shared.Utils;

namespace Shared.Core.Async
{
    public class MultiAsyncOperation
    {
        private readonly HashSet<IAsyncOperation> _operations = new();

        public MultiAsyncOperation Add(params IAsyncOperation[] o)
        {
            _operations.AddRange(o);
            return this;
        }

        public bool IsAllOperationComplete() => _operations.All(o => o.IsComplete);
        public bool IsAllOperationSuccess() => _operations.All(o => o.IsComplete && o.IsSuccess);
        public bool IsAllOperationFailed() => _operations.All(o => o.IsComplete && !o.IsSuccess);

        public bool IsAnyOperationComplete() => _operations.Any(o => o.IsComplete);
        public bool IsAnyOperationSuccess() => _operations.Any(o => o.IsComplete && o.IsSuccess);
        public bool IsAnyOperationFailed() => _operations.Any(o => o.IsComplete && !o.IsSuccess);
    }
}