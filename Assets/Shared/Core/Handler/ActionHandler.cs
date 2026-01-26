using System;

namespace Shared.Core.Handler
{
    public class ActionHandler : IHandler
    {
        private readonly Action _action;

        public ActionHandler(Action action)
        {
            _action = action;
        }

        public void Handle() => _action.Invoke();
    }
    
    public class ActionHandler<T> : IHandler<T>
    {
        private readonly Action<T> _action;

        public ActionHandler(Action<T> action)
        {
            _action = action;
        }

        public void Handle(T t) => _action.Invoke(t);
    }
}