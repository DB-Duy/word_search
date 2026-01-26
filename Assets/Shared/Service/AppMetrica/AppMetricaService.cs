#if APPMETRICA
using System.Diagnostics;
using Io.AppMetrica;
using Io.AppMetrica.Profile;
using Shared.Core.Async;
using Shared.Core.IoC;
using Shared.Core.IoC.UnityLifeCycle;
using Shared.Entity.Config;
using Shared.Repository.AppMetrica;
using Shared.Utils;
using UnityEngine;
using Zenject;

namespace Shared.Service.AppMetrica
{
    [Service]
    public class AppMetricaService: IAppMetricaService, ISharedUtility
    {
        private const int DefaultSessionTimeOut = 300;

        [Inject] private AppMetricaRepository _appMetricaRepository;
        [Inject] private IConfig _config;
        
        public string ProfileId { get; private set; }

        private bool _actualPauseStatus;
        
        public bool IsInitialized { get; private set; }
        private IAsyncOperation _initOperation;
        public IAsyncOperation Initialize()
        {
            this.LogInfo(SharedLogTag.AppMetrica);
            if (_initOperation != null) return _initOperation;
            
            if (Application.isEditor) return new SharedAsyncOperation().Success();

            ProfileId = SystemInfo.deviceUniqueIdentifier;
            
            this.LogInfo(SharedLogTag.AppMetrica, "reach", "ProfileId = SystemInfo.deviceUniqueIdentifier;");
            
            var e = _appMetricaRepository.Get();
            e.UserProfileID = ProfileId;
            _appMetricaRepository.Save(e);
            
            var config = new Io.AppMetrica.AppMetricaConfig(_config.AppmetricaId)
            {
                SessionTimeout = 300,
                Logs = SharedSymbols.IsDevelopment,
                RevenueAutoTrackingEnabled = false,
                SessionsAutoTrackingEnabled = true,
                MaxReportsCount = 1,
                MaxReportsInDatabaseCount = 10000,
                UserProfileID = SystemInfo.deviceUniqueIdentifier
            };
            this.LogInfo(SharedLogTag.AppMetrica, nameof(config), config);
            Io.AppMetrica.AppMetrica.OnActivation += (c) =>
            {
                this.LogInfo(SharedLogTag.AppMetrica, "f", "Io.AppMetrica.AppMetrica.OnActivation", nameof(c), c);
            };
            Io.AppMetrica.AppMetrica.Activate(config);
            
            // https://appmetrica.yandex.com/docs/en/sdk/android/analytics/android-operations#track-adv-identifiers
            var userProfile = new UserProfile().Apply(Attribute.CustomNumber("deviceRAM").WithValue(SystemInfo.systemMemorySize));
            // Setting the ProfileID using the method of the AppMetrica class.
            Io.AppMetrica.AppMetrica.SetUserProfileID(SystemInfo.deviceUniqueIdentifier);
            Io.AppMetrica.AppMetrica.ReportUserProfile(userProfile);
            this.LogInfo(SharedLogTag.AppMetrica, "deviceRAM", SystemInfo.systemMemorySize);
            _DebugAppMetrica();
            IsInitialized = true;
            _initOperation = new SharedAsyncOperation().Success();
            return _initOperation;
        }

        [Conditional("LOG_INFO")]
        private void _DebugAppMetrica()
        {
            if(Application.isEditor) return;
            var identifiers = new[] { StartupParamsKey.AppMetricaDeviceID, StartupParamsKey.AppMetricaUuid, StartupParamsKey.AppMetricaDeviceIDHash };
            var i = this;
            Io.AppMetrica.AppMetrica.RequestStartupParams((result, reason) =>
            {
                i.LogInfo(SharedLogTag.AppMetrica, nameof(result), result?.Parameters, nameof(reason), reason);
            }, identifiers);
        }
    }
}
#endif