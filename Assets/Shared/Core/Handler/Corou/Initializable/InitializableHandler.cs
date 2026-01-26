using System.Collections;

namespace Shared.Core.Handler.Corou.Initializable
{
    public class InitializableHandler : IInitializableHandler
    {
        public Config Config { get; }
        private readonly IInitializable _initializable;

        public InitializableHandler(IInitializable initializable, int timeout = 0, bool isFreeTask = false)
        {
            _initializable = initializable;
            Config = new Config()
            {
                Name = initializable.GetType().Name,
                TimeOut = timeout,
                IsFreeTask = isFreeTask
            };
        }

        public IEnumerator Handle()
        {
            if(_initializable == null)
                yield break;

            yield return Config.Handle(_initializable);
        }
    }
}