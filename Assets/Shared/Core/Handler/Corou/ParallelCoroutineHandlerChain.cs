using System.Collections;
using System.Linq;
using Shared.Core.IoC;
using Shared.Service.SharedCoroutine;

namespace Shared.Core.Handler.Corou
{
    public class ParallelCoroutineHandlerChain : ICoroutineHandler, ISharedUtility
    {
        private ICoroutineHandler[] _handlers;

        public ParallelCoroutineHandlerChain(params ICoroutineHandler[] handlers)
        {
            _handlers = handlers;
        }

        public IEnumerator Handle()
        {
            foreach (var handler in _handlers)
                this.StartSharedCoroutine(handler.Handle());
            yield break;
        }
        
        public static ParallelCoroutineHandlerChain CreateChainFromType<C>()
        {
            var items = IoCExtensions.ResolveAll<C>();
            return new ParallelCoroutineHandlerChain
            {
                _handlers = items.OfType<ICoroutineHandler>().ToArray()
            };
        }
    }
}