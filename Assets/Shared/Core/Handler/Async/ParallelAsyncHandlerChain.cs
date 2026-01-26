using System;
using System.Collections;
using Shared.Core.Async;
using Shared.Service.SharedCoroutine;

namespace Shared.Core.Handler.Async
{
    public class ParallelAsyncHandlerChain : IAsyncHandler, ISharedUtility
    {
        private readonly IAsyncHandler[] _handlers;

        public ParallelAsyncHandlerChain(params IAsyncHandler[] handlers)
        {
            _handlers = handlers;
        }

        public IAsyncOperation Handle()
        {
            var operation = new SharedAsyncOperation();
            this.StartSharedCoroutine(_Handle(() => operation.Success()));
            return operation;
        }

        private IEnumerator _Handle(Action onComplete)
        {
            var multiAsyncOperation = new MultiAsyncOperation();
            
            foreach (var h in _handlers)
                multiAsyncOperation.Add(h.Handle());

            while (!multiAsyncOperation.IsAllOperationComplete())
                yield return null;
            
            onComplete.Invoke();
        }
    }
    
    public class ParallelAsyncHandlerChain<T> : IAsyncHandler<T>, ISharedUtility
    {
        private readonly IAsyncHandler<T>[] _handlers;

        public ParallelAsyncHandlerChain(params IAsyncHandler<T>[] handlers)
        {
            _handlers = handlers;
        }

        public IAsyncOperation Handle(T apsSlotId)
        {
            var operation = new SharedAsyncOperation();
            this.StartSharedCoroutine(_Handle(apsSlotId, () => operation.Success()));
            return operation;
        }

        private IEnumerator _Handle(T t, Action onComplete)
        {
            var multiAsyncOperation = new MultiAsyncOperation();
            
            foreach (var h in _handlers)
                multiAsyncOperation.Add(h.Handle(t));

            while (!multiAsyncOperation.IsAllOperationComplete())
                yield return null;
            
            onComplete.Invoke();
        }
    }
}