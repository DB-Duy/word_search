using System;
using Shared.Core.IoC.UnityLifeCycle;
using Shared.Core.Repository.RemoteConfig;
using Shared.Entity.Config;
using Shared.Repository.Config;
using Shared.Service.FirebaseRemoteConfig;
using Shared.Service.SharedCoroutine;
using Shared.Service.Tracking;
using Shared.Utils;
using UnityEngine;
using Zenject;

namespace Shared.Core.IoC
{
    public class ProjectInstaller : MonoInstaller, ISharedUtility
    {
        private const string Tag = "ProjectInstaller";
        private GameObject _dontDestroyOnLoadHolder;
        private UnityLifeCycle.UnityLifeCycle _unityLifeCycle;
        public static IConfig Config { get; private set; }

        public override void InstallBindings()
        {
            IoCExtensions.Instance = Container;

            _dontDestroyOnLoadHolder = new GameObject();
            _unityLifeCycle = _dontDestroyOnLoadHolder.AddComponent<UnityLifeCycle.UnityLifeCycle>();
            SharedCoroutineServiceExtensions.CoroutineMonoBehaviour = _unityLifeCycle;
            SharedUtilities.SharedGameObject = _dontDestroyOnLoadHolder;
            DontDestroyOnLoad(_dontDestroyOnLoadHolder);

            // -----------------------------------------------------------------------------
            // Bind Config
            // -----------------------------------------------------------------------------
            var configRepository = new ConfigRepository();
            var config = configRepository.Get();
            Config = config;
            Container.BindInterfacesAndSelfTo<Config>().FromInstance(config).AsSingle();
            QualitySettings.vSyncCount = 0; // Set vSyncCount to 0 so that using .targetFrameRate is enabled.
            Application.targetFrameRate = config.TargetFrameRate;
            this.LogInfo("Application.targetFrameRate", Application.targetFrameRate, "config.TargetFrameRate", config.TargetFrameRate);

            // Container.Bind<SignalBus>().AsSingle();
            SignalBusInstaller.Install(Container);
            
            // -----------------------------------------------------------------------------
            // Clean Architecture Attributes.
            // -----------------------------------------------------------------------------
            var signalEventAttributeType = typeof(SignalEventAttribute);
            var componentAttributeType = typeof(ComponentAttribute);
            var repositoryAttributeType = typeof(RepositoryAttribute);
            var serviceAttributeType = typeof(ServiceAttribute);
            
            var types = GetType().Assembly.GetTypes();
            SharedLogger.Log($"service types count {types.Length}");
            foreach (var type in types)
            {
                if (TryBindType(type, componentAttributeType)) continue;
                if (TryBindType(type, repositoryAttributeType)) continue;
                if (TryBindType(type, serviceAttributeType)) continue;
                if (TryDeclareSignal(type, signalEventAttributeType)) continue;
            }
        }

        private bool TryDeclareSignal(Type type, Type attributeType)
        {
            if (!type.Name.EndsWith("Signal")) return false;
            var attributes = type.GetCustomAttributes(attributeType, false);
            if (attributes.Length <= 0) return false;
            Container.DeclareSignal(type);
            return true;
        }

        private bool TryBindType(Type type, Type attributeType)
        {
            var attributes = type.GetCustomAttributes(attributeType, false);
            if (attributes.Length <= 0) return false;
            var a0 = (BaseAttribute) attributes[0];
#if LOG_INFO
            if (typeof(MonoBehaviour).IsAssignableFrom(type)) throw new Exception($"{type.FullName} is MonoBehaviour");
#endif
            var isInitializable = typeof(Shared.Core.Handler.Corou.Initializable.IInitializable).IsAssignableFrom(type);
            this.LogInfo(nameof(type), type.FullName, nameof(isInitializable), isInitializable);
            if (a0.Lazy && !isInitializable) Container.BindInterfacesAndSelfTo(type).AsSingle().OnInstantiated(_OnInstantiated).Lazy();
            else Container.BindInterfacesAndSelfTo(type).AsSingle().OnInstantiated(_OnInstantiated).NonLazy();
            return true;
        }

        private void _OnInstantiated(InjectContext context, object o)
        {
            this.LogInfo(nameof(o), o.GetType().FullName);
            if (o is IUnityUpdate update) 
                _unityLifeCycle.SequenceUnityUpdate.Add(update);
            if (o is IUnityOnApplicationPause applicationPause) 
                _unityLifeCycle.SequenceUnityOnApplicationPause.Add(applicationPause);
            if (o is Shared.Core.Handler.Corou.Initializable.IInitializable initializable)
                InitializableRegistry.Register(initializable.GetType().FullName, initializable);
            if (o is ITrackingService trackingService)
                TrackingUtility.TrackingService = trackingService;
            if (o is IFirebaseRemoteConfigRepository firebaseRemoteConfigRepository)
                FirebaseRemoteConfigRegistry.Register(firebaseRemoteConfigRepository);
        }
    }
}