using System;
using System.Collections;

namespace Shared.Core.Handler.Corou.Initializable
{
    public class ActionInitializableHandler : IInitializableHandler
    {
        public Config Config { get; }
        private readonly Action _action;

        public ActionInitializableHandler(string name, Action action)
        {
            _action = action;
            Config = new Config()
            {
                Name = name,
                TimeOut = 0,
                IsFreeTask = false
            };
        }

        public IEnumerator Handle()
        {
            _action.Invoke();
            yield break;
        }
    }
}