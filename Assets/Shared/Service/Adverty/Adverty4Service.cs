#if ADVERTY && ADVERTY_4
using System.Runtime.CompilerServices;
using Adverty;
using Adverty.AdDebug;
using Shared.Core.Async;
using Shared.Core.IoC;
using Shared.Core.IoC.UnityLifeCycle;
using Shared.Entity.Adverty;
using Shared.Entity.Config;
using Shared.Repository.InPlayAds;
using Shared.Service.InPlayAds;
using Shared.Utils;
using UnityEngine;
using Zenject;

namespace Shared.Service.Adverty
{
    [Service]
    public class Adverty4Service : IAdvertyService, ISharedUtility, IUnityUpdate
    {
        [Inject] private IConfig _config;
        [Inject] private InPlayAdsConfigRepository _inplayAdsConfigRepository;

        public bool IsInitialized { get; private set; } = false;
        private IAsyncOperation _initOperation;
        
        private IAsyncOperation _advertyOperation;

        public IAsyncOperation Initialize()
        {
            if (_initOperation != null) return _initOperation;
            _initOperation = new SharedAsyncOperation().Success();
            IsInitialized = true;
            // Add listeners
            AdvertyEvents.AdDelivered += _OnAdDelivered;
            AdvertyEvents.UnitActivated += _OnUnitActivated;
            AdvertyEvents.UnitDeactivated += _OnUnitDeactivated;
            AdvertyEvents.UnitActivationFailed += _OnUnitActivationFailed;
            AdvertyEvents.AdvertySessionActivated += _OnAdvertySessionActivated;
            AdvertyEvents.AdvertySessionActivationFailed += _OnAdvertySessionActivationFailed;
            AdvertyEvents.AdvertySessionTerminated += _OnAdvertySessionTerminated;

            // Settings
            var isDevelopment = SharedSymbols.IsDevelopment;
            AdvertySettings.EnableLogs = isDevelopment;
            AdvertySettings.AdvertyLogLevel = isDevelopment ? LogLevel.Debug : LogLevel.None;
            AdvertySettings.SandboxMode = false;

            var config = _inplayAdsConfigRepository.Get();
            var unlocked = config == null || config.Unlocked;
            if (unlocked) _InitAdverty();
            
            return _initOperation;
        }

        private void _InitAdverty()
        {
            if (_advertyOperation != null) return;
            var userData = new UserData(AgeSegment.Unknown, Gender.Unknown);
            this.LogInfo(SharedLogTag.InPlayAdsNAdverty, nameof(_config.AdvertyApiKey), _config.AdvertyApiKey, nameof(userData), userData);
            _advertyOperation = new SharedAsyncOperation();
            AdvertySDK.Init(apiKey: _config.AdvertyApiKey, platform: AdvertySettings.Mode.Mobile, restrictUserData: false, userData: userData);
        }

        public void SetMainCamera(Camera camera)
        {
            AdvertySettings.SetMainCamera(camera);
        }

        public AdvertyEntity Get()
        {
            return new AdvertyEntity
            {
                APIKey = AdvertySettings.APIKey,
                EnableLogs = AdvertySettings.EnableLogs,
                LogLevel = AdvertySettings.AdvertyLogLevel.ToString(),
                ApiVersion = AdvertySettings.API_VERSION,
                Version = AdvertySettings.VERSION,
                SandboxMode = AdvertySettings.SandboxMode
            };
        }

        private void _Terminate()
        {
            if (_advertyOperation == null) return;
            this.LogInfo(SharedLogTag.InPlayAdsNAdverty);
            AdvertySDK.Terminate();
        }

        public void Update()
        {
            if (Time.frameCount % 60 != 0) return;
            var config = _inplayAdsConfigRepository.Get();
            var unlocked = config == null || config.Unlocked;
            if (unlocked) _InitAdverty(); else _Terminate();
        }

        // -------------------------------------------------------------------------------------------------------------
        // Version 4 Callbacks
        // -------------------------------------------------------------------------------------------------------------
        private void _OnAdDelivered(BaseUnit unit)
        {
            this.LogInfo(SharedLogTag.InPlayAdsNAdverty, "f", nameof(_OnAdDelivered), nameof(unit.name), unit.name);
            InPlayAdRegistry.RegisterPotentialProvider(InPlayAdProvider.Adverty);
        }

        private void _OnUnitActivated(BaseUnit unit)
        {
            this.LogInfo(SharedLogTag.InPlayAdsNAdverty, "f", nameof(_OnUnitActivated), nameof(unit.name), unit.name);
        }

        private void _OnUnitDeactivated(BaseUnit unit)
        {
            this.LogInfo(SharedLogTag.InPlayAdsNAdverty, "f", nameof(_OnUnitDeactivated), nameof(unit.name), unit.name);
        }

        private void _OnUnitActivationFailed(BaseUnit unit)
        {
            this.LogInfo(SharedLogTag.InPlayAdsNAdverty, "f", nameof(_OnUnitActivationFailed), nameof(unit.name), unit.name);
        }

        private void _OnAdvertySessionActivated()
        {
            this.LogInfo(SharedLogTag.InPlayAdsNAdverty, "f", nameof(_OnAdvertySessionActivated));
        }

        private void _OnAdvertySessionActivationFailed()
        {
            this.LogInfo(SharedLogTag.InPlayAdsNAdverty, "f", nameof(_OnAdvertySessionActivationFailed));
        }

        private void _OnAdvertySessionTerminated()
        {
            this.LogInfo(SharedLogTag.InPlayAdsNAdverty, "f", nameof(_OnAdvertySessionTerminated));
        }
    }
}
#endif