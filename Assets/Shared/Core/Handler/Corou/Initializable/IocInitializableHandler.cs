using System.Collections;
using System.Collections.Generic;
using Shared.Core.IoC;

namespace Shared.Core.Handler.Corou.Initializable
{
    public class IocInitializableHandler<T> : IInitializableHandler, IIoC where T : class, IInitializable
    {
        public Config Config { get; }
        
        public IocInitializableHandler(bool optional = false, int timeOut = 0, bool freeTask = false)
        {
            Config = new Config()
            {
                Optional = optional,
                Name = typeof(T).Name,
                TimeOut = timeOut,
                IsFreeTask = freeTask
            };
        }

        public IEnumerator Handle()
        {
            var i = Config.Optional ? this.TryResolve<T>() : this.Resolve<T>();
            if (i == null) yield break;
            yield return Config.Handle(i);
        }    
    }
    
    public class IocInitializableHandler<T, R> : IInitializableHandler, IIoC 
        where T : class, IInitializable
        where R : class, IInitializable
    {
        public Config Config { get; }
        
        // Error FA Name must consist of letters, digits or _ (underscores). Type, name: event param, IAdjustService&IUnityGamingService
        public IocInitializableHandler(bool optional = false, int timeOut = 0, bool freeTask = false)
        {
            Config = new Config()
            {
                Optional = optional,
                Name = $"{typeof(T).Name}_{typeof(R).Name}",
                TimeOut = timeOut,
                IsFreeTask = freeTask
            };
        }

        public IEnumerator Handle()
        {
            var list = new List<IInitializable>();
            var t = Config.Optional ? this.TryResolve<T>() : this.Resolve<T>();
            var r = Config.Optional ? this.TryResolve<R>() : this.Resolve<R>();
            if (t == null && r == null) yield break;
            if (t != null) list.Add(t);
            if (r != null) list.Add(r);
            yield return Config.Handle(list.ToArray());
        }    
    }
    
    public class IocInitializableHandler<T, R, L> : IInitializableHandler, IIoC 
        where T : IInitializable
        where R : IInitializable
        where L : IInitializable
    {
        private const string Tag = "IocInitializableHandler";
        public Config Config { get; }
        
        public IocInitializableHandler(int timeOut = 0, bool freeTask = false)
        {
            Config = new Config()
            {
                Name = $"{typeof(T).Name}_{typeof(R).Name}_{typeof(L).Name}",
                TimeOut = timeOut,
                IsFreeTask = freeTask
            };
        }

        public IEnumerator Handle()
        {
            var t = this.Resolve<T>();
            var r = this.Resolve<R>();
            var l = this.Resolve<L>();
            yield return Config.Handle(t, r, l);
        }    
    }
}