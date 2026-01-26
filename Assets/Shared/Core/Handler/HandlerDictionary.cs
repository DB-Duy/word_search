using System.Collections.Generic;
using System.Linq;
using Shared.Core.IoC;

namespace Shared.Core.Handler
{
    public class HandlerDictionary : IHandler<string>
    {
        private readonly Dictionary<string, IHandler> _handlers = new();

        public HandlerDictionary(Dictionary<string, IHandler> handlers)
        {
            _handlers = handlers;
        }
        
        public HandlerDictionary(string id1, IHandler handler1, string id2 = null, IHandler handler2 = null, string id3 = null, IHandler handler3 = null)
        {
            _handlers.Add(id1, handler1);
            if (id2 != null) _handlers.Add(id2, handler2);
            if (id3 != null) _handlers.Add(id3, handler3);
        }

        public void Handle(string t)
        {
            if (string.IsNullOrEmpty(t))
            {
                foreach (var h in _handlers) 
                    h.Value.Handle();
                return;
            }
            _handlers[t].Handle();
        }
        
        public static HandlerDictionary CreateDictionaryFromType<C>() where C : IHandler, IHandlerKey
        {
            var items = IoCExtensions.ResolveAll<C>();
            var handlers = items.ToDictionary<C, string, IHandler>(item => item.Key, item => item);
            return new HandlerDictionary(handlers);
        }
        
    }
}