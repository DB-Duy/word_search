using System.Collections.Generic;
using System.Linq;
using Shared.Core.IoC;

namespace Shared.Core.Handler
{
    public class SequenceHandlerChain : IHandler
    {
        private IHandler[] _handlers;

        public SequenceHandlerChain(params IHandler[] handlers)
        {
            _handlers = handlers;
        }

        public void Handle()
        {
            foreach (var h in _handlers) h.Handle();
        }
        
        public static SequenceHandlerChain Create<C1, C2>() where C1 : IHandler where C2 : IHandler
        {
            return new SequenceHandlerChain
            (
                IoCExtensions.Resolve<C1>(),
                IoCExtensions.Resolve<C2>()
            );
        }
        
        public static SequenceHandlerChain Create<C1, C2, C3>() where C1 : IHandler where C2 : IHandler where C3 : IHandler
        {
            return new SequenceHandlerChain
            (
                IoCExtensions.Resolve<C1>(),
                IoCExtensions.Resolve<C2>(),
                IoCExtensions.Resolve<C3>()
            );
        }
        
        public static SequenceHandlerChain CreateChainFromType<C>()
        {
            var items = IoCExtensions.ResolveAll<C>();
            return new SequenceHandlerChain
            {
                _handlers = items.OfType<IHandler>().ToArray()
            };
        }
    }
    
    public class SequenceHandlerChain<T> : IHandler<T>
    {
        private IHandler<T>[] _handlers;

        public SequenceHandlerChain(params IHandler<T>[] handlers)
        {
            _handlers = handlers;
        }

        public void Handle(T t)
        {
            foreach (var h in _handlers) 
                h.Handle(t);
        }

        public static SequenceHandlerChain<T> Create<C1, C2>() 
            where C1 : IHandler<T>
            where C2 : IHandler<T>
        {
            return new SequenceHandlerChain<T>
            (
                IoCExtensions.Resolve<C1>(),
                IoCExtensions.Resolve<C2>()
            );
        }
        
        public static SequenceHandlerChain<T> Create<C1, C2, C3>() 
            where C1 : IHandler<T>
            where C2 : IHandler<T>
            where C3 : IHandler<T>
        {
            return new SequenceHandlerChain<T>
            (
                IoCExtensions.Resolve<C1>(),
                IoCExtensions.Resolve<C2>(),
                IoCExtensions.Resolve<C3>()
            );
        }
        
        public static SequenceHandlerChain<T> Create<C1, C2, C3, C4>() 
            where C1 : IHandler<T>
            where C2 : IHandler<T>
            where C3 : IHandler<T>
            where C4 : IHandler<T>
        {
            return new SequenceHandlerChain<T>
            (
                IoCExtensions.Resolve<C1>(),
                IoCExtensions.Resolve<C2>(),
                IoCExtensions.Resolve<C3>(),
                IoCExtensions.Resolve<C4>()
            );
        }
        
        public static SequenceHandlerChain<T> Create<C1, C2, C3, C4, C5, C6>() 
            where C1 : IHandler<T>
            where C2 : IHandler<T>
            where C3 : IHandler<T>
            where C4 : IHandler<T>
            where C5 : IHandler<T>
            where C6 : IHandler<T>
        {
            return new SequenceHandlerChain<T>
            (
                IoCExtensions.Resolve<C1>(),
                IoCExtensions.Resolve<C2>(),
                IoCExtensions.Resolve<C3>(),
                IoCExtensions.Resolve<C4>(),
                IoCExtensions.Resolve<C5>(),
                IoCExtensions.Resolve<C6>()
            );
        }
        
        public static SequenceHandlerChain<T> CreateChainFromType<C>()
        {
            var items = IoCExtensions.ResolveAll<C>();
            return new SequenceHandlerChain<T>
            {
                _handlers = items.OfType<IHandler<T>>().ToArray()
            };
        }
    }
    
    public class SequenceHandlerChain<T, R> : IHandler<T, R>
    {
        private IHandler<T, R>[] _handlers;

        public SequenceHandlerChain(params IHandler<T, R>[] handlers)
        {
            _handlers = handlers;
        }

        public void Handle(T t, R r)
        {
            foreach (var h in _handlers) 
                h.Handle(t, r);
        }
        
        public static SequenceHandlerChain<T, R> CreateChainFromType<C>()
        {
            var items = IoCExtensions.ResolveAll<C>();
            return new SequenceHandlerChain<T, R>
            {
                _handlers = items.OfType<IHandler<T, R>>().ToArray()
            };
        }
    }
}