#if ADVERTY_5
using System.Collections.Generic;
using Adverty5;
using AOT;
using Newtonsoft.Json;
using Shared.Core.Async;
using Shared.Core.IoC;
using Shared.Entity.Config;
using Shared.Service.InPlayAds;
using Shared.Service.ParentControl;
using Shared.Service.Tracking;
using Shared.Tracking.Models.Ads;
using Shared.Utilities;
using Shared.Utils;
using Zenject;

namespace Shared.Service.Adverty
{
    /// <summary>
    /// https://adverty.com/sdk-5-upgrade-guide/
    ///
    /// Adverty failed to sign in with HTTP error code: 0x72c09da320. Please check the API key or try again later.
    /// => Resolve: Mở VPN sang US.
    /// </summary>
    [Service]
    public class Adverty5Service : IAdvertyService, ISharedLogTag, ISharedUtility
    {
        public string LogTag => SharedLogTag.InPlayAdsNAdverty;

        private static Dictionary<Shared.Entity.ParentControl.Gender, Gender> GenderMap = new()
        {
            {Shared.Entity.ParentControl.Gender.Male, Gender.Male},
            {Shared.Entity.ParentControl.Gender.Female, Gender.Female},
            {Shared.Entity.ParentControl.Gender.Other, Gender.Other},
            {Shared.Entity.ParentControl.Gender.Unknown, Gender.Unknown},
        };

        [Inject] private IConfig _config;
        [Inject(Optional = true)] private IParentControlService _parentControlService;

        public bool IsInitialized { get; private set; }
        private IAsyncOperation _initOperation;

        public IAsyncOperation Initialize()
        {
            if (_initOperation != null) return _initOperation;
            _initOperation = new SharedAsyncOperation().Success();
            IsInitialized = true;

            var sessionCallbacks = new SessionCallbacks();
            sessionCallbacks.OnPreviewOpenedCallback += _OnPreviewOpenedCallback;
            sessionCallbacks.OnPreviewClosedCallback += _OnPreviewClosedCallback;
            sessionCallbacks.OnAdViewedCallback += _OnAdViewedCallback;

            sessionCallbacks.OnPaidCallback += _OnPaidCallback;
            sessionCallbacks.OnBrowserOpenCallback += _OnBrowserOpenCallback;
            sessionCallbacks.OnBrowserClosedCallback += _OnBrowserClosedCallback;

            var userData = new UserData
            { 
                consented = true
            };

            if (_parentControlService != null)
            {
                var parentControlEntity = _parentControlService.Get();
                userData.gender = GenderMap.Get(parentControlEntity.Gender, Gender.Undefined);
                userData.yearOfBirth = parentControlEntity.YearOfBirth;
            }

            var data = new LaunchData
            {
                callbacks = sessionCallbacks,
                userData = userData,
                apiKey = _config.Adverty5ApiKey,
#if UNITY_EDITOR
                sandboxMode = SharedSymbols.IsDevelopment
#endif
            };

            Adverty5.Adverty.Start(data);
            this.LogInfo($"Adverty 5 Initialized: {JsonConvert.SerializeObject(data)}");
            return _initOperation;
        }

        [MonoPInvokeCallback(typeof(PreviewOpenedCallback))]
        private static void _OnPreviewOpenedCallback(int x, int y, int width, int height)
        {
            SharedLogger.LogInfoCustom(SharedLogTag.InPlayAdsNAdverty, nameof(Adverty5Service), "_OnPreviewOpenedCallback", nameof(x), x, nameof(y), y, nameof(width), width, nameof(height), height);
        }

        [MonoPInvokeCallback(typeof(PreviewClosedCallback))]
        private static void _OnPreviewClosedCallback()
        {
            SharedLogger.LogInfoCustom(SharedLogTag.InPlayAdsNAdverty, nameof(Adverty5Service), "_OnPreviewClosedCallback");
        }

        [MonoPInvokeCallback(typeof(AdViewedCallback))]
        private static void _OnAdViewedCallback(int placementId)
        {
            SharedLogger.LogInfoCustom(SharedLogTag.InPlayAdsNAdverty, nameof(Adverty5Service), "_OnAdViewedCallback", nameof(placementId), placementId);
        }

        [MonoPInvokeCallback(typeof(PaidCallback))]
        private static void _OnPaidCallback(double price)
        {
            var adverty = IoCExtensions.Instance.Resolve<Adverty5Service>();
            adverty.LogInfo(SharedLogTag.InPlayAdsNAdverty, "keyword", "impression Adverty", "f", nameof(_OnPaidCallback), nameof(price), price);
            var usd = price / 1000.0;
            adverty.Track(new AdRevenueEvent(
                adPlatform: "adverty",
                adSource: "adverty",
                adUnitName: null,
                adFormat: "in_play",
                currency: "USD",
                value: usd));
            InPlayAdRegistry.RegisterPotentialProvider(InPlayAdProvider.Adverty);
        }

        [MonoPInvokeCallback(typeof(BrowserOpenCallback))]
        private static void _OnBrowserOpenCallback()
        {
            var adverty = IoCExtensions.Instance.Resolve<Adverty5Service>();
            adverty.LogInfo();
        }

        [MonoPInvokeCallback(typeof(BrowserClosedCallback))]
        private static void _OnBrowserClosedCallback()
        {
            var adverty = IoCExtensions.Instance.Resolve<Adverty5Service>();
            adverty.LogInfo();
        }
    }
}
#endif