using System;
using System.Collections;
using Shared.Core.Async;
using Shared.Service.SharedCoroutine;

namespace Shared.Core.Handler.Async
{
    
    public class SequenceAsyncHandlerChain : IAsyncHandler, ISharedUtility
    {
        private readonly IAsyncHandler[] _handlers;

        public SequenceAsyncHandlerChain(params IAsyncHandler[] handlers)
        {
            _handlers = handlers;
        }

        public IAsyncOperation Handle()
        {
            var operation = new SharedAsyncOperation();
            this.StartSharedCoroutine(_Handle((reason) => operation.Fail(reason), () => operation.Success()));
            return operation;
        }

        private IEnumerator _Handle(Action<string> onFail, Action onComplete)
        {
            foreach (var h in _handlers)
            {
                var o = h.Handle();
                while (!o.IsComplete) yield return null;
            }
            onComplete.Invoke();
        }
    }
    
    public class SequenceAsyncHandlerChain<T> : IAsyncHandler<T>, ISharedUtility
    {
        private readonly IAsyncHandler<T>[] _handlers;

        public SequenceAsyncHandlerChain(params IAsyncHandler<T>[] handlers)
        {
            _handlers = handlers;
        }

        public IAsyncOperation Handle(T apsSlotId)
        {
            var operation = new SharedAsyncOperation();
            this.StartSharedCoroutine(_Handle(apsSlotId, (reason) => operation.Fail(reason), () => operation.Success()));
            return operation;
        }

        private IEnumerator _Handle(T t, Action<string> onFail, Action onComplete)
        {
            foreach (var h in _handlers)
            {
                var o = h.Handle(t);
                while (!o.IsComplete) yield return null;
            }
            onComplete.Invoke();
        }
    }
}