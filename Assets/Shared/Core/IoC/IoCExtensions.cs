using System;
using System.Collections.Generic;
using Zenject;

namespace Shared.Core.IoC
{
    public static class IoCExtensions
    {
        public static DiContainer Instance { get; set; }
        public static DiContainer SceneInstance { get; set; }

        public static T Resolve<T>() => Instance == null ? default : Instance.Resolve<T>();
        public static T Resolve<T>(this IIoC o) => Instance == null ? default : Instance.Resolve<T>();
        public static List<T> ResolveAll<T>() => Instance?.ResolveAll<T>();
        
        public static T TryResolve<T>() where T : class => Instance?.TryResolve<T>();
        public static T TryResolve<T>(this IIoC o) where T : class => Instance?.TryResolve<T>();
        
        public static void InjectDependencies(this IIoC obj)
        {
            bool failed = false;
            Exception error = null;
            try
            {
                Instance?.Inject(obj);
            }
            catch (Exception e)
            {
                // ignored
            }

            try
            {
                SceneInstance?.Inject(obj);
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        
        public static void Bind<T>(this IIoC ioc, T o)
        {
            var type = typeof(T);
            Instance.BindInterfacesAndSelfTo(type).FromInstance(o).AsSingle();
        }
    }
}