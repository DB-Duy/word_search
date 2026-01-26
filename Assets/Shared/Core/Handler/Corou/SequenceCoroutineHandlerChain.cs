using System.Collections;

namespace Shared.Core.Handler.Corou
{
    public class SequenceCoroutineHandlerChain : ICoroutineHandler, ISharedUtility
    {
        private readonly ICoroutineHandler[] _handlers;

        public SequenceCoroutineHandlerChain(params ICoroutineHandler[] handlers)
        {
            _handlers = handlers;
        }

        public IEnumerator Handle()
        {
            foreach (var handler in _handlers)
                yield return handler.Handle();
        }
    }
}