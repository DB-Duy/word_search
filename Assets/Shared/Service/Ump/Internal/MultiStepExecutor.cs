#if USING_UMP
using System.Collections;
using Shared.Core.Async;
using Shared.Service.SharedCoroutine;
using Shared.Utilities;
using Shared.Utils;
using UnityEngine;

namespace Shared.Service.Ump.Internal
{
    public class MultiStepExecutor : IStepExecutor, ISharedUtility, ISharedLogTag
    {
        private readonly IStepExecutor[] _executors;
        private IAsyncOperation _operation;

        public MultiStepExecutor(params IStepExecutor[] executors)
        {
            _executors = executors;
        }

        public IAsyncOperation Execute(UmpProcessInfo i)
        {
            if (_operation != null) return _operation;
            _operation = new SharedAsyncOperation();
            this.StartSharedCoroutine(_Execute(i));
            return _operation;
        }

        private IEnumerator _Execute(UmpProcessInfo i)
        {
            foreach (var e in _executors)
            {
                var r = e.Execute(i);
                yield return new WaitUntil(() => r.IsComplete);
                if (r.IsSuccess) continue;
                this.LogWarning("task", e.GetType().Name, nameof(r.FailReason), r.FailReason);
                _operation.Fail($"Failed at {e.GetType()}->Fail reason: {r.FailReason}");
                break;
            }
            _operation.Success();
            i.OnComplete?.Invoke();
            this.LogInfo("result", "Successed");
        }

        public string LogTag => SharedLogTag.Ump;
    }
}
#endif