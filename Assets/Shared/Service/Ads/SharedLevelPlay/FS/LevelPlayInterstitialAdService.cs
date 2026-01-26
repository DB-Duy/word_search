#if LEVEL_PLAY && !UNITY_EDITOR
using System;
using System.Collections.Generic;
using Shared.Core.Async;
using Shared.Core.Handler.Async;
using Shared.Core.IoC;
using Shared.Core.IoC.UnityLifeCycle;
using Shared.Core.Validator;
using Shared.Entity.Ads;
using Shared.Entity.Config;
using Shared.Repository.Ads;
using Shared.Service.Ads.Common;
using Shared.Service.Ads.Validator;
using Shared.Service.Aps;
using Shared.Service.NotificationCenter;
using Shared.Service.Tracking;
using Shared.Tracking.Models.Ads;
using Shared.Utilities;
using Shared.Utils;
using UnityEngine;
using Zenject;
using TimeOut = Shared.Core.Async.TimeOut;

namespace Shared.Service.Ads.SharedLevelPlay.FS
{
    /// <summary>
    /// https://developers.is.com/ironsource-mobile/unity/interstitial-integration-unity/#step-6
    /// </summary>
    [Service]
    public class LevelPlayInterstitialAdService : IUnityUpdate, IInterstitialAd, ISharedUtility, ISharedLogTag, IUnityOnApplicationPause
    {
        public string LogTag => SharedLogTag.AdNLevelPlayNInterstitial_;

        [Inject] private IAdService _adService;
        [Inject] private IConfig _config;
        
        [Inject] private InterstitialAdRepository _interstitialAdRepository;
        [Inject] private MultipleInterstitialAdsConfigRepository _multipleInterstitialAdsConfigRepository;
        [Inject] private AdsBreakConfigRepository _adsBreakConfigRepository;

        private IValidator _loadValidator;
        private IValidator LoadValidator => _loadValidator ??= ValidatorChain.CreateChainFromType<IInterstitialLoadValidator>();

        private IValidator<string> _placementValidator;
        private IValidator<string> PlacementValidator => _placementValidator ??= ValidatorChain<string>.CreateChainFromType<IInterstitialShowValidator>();

#if LEVEL_PLAY && APS
        private IAsyncHandler<ApsInterstitialConfig> _apsInterstitialFetcher;
        private IAsyncHandler<ApsInterstitialConfig> ApsInterstitialFetcher => _apsInterstitialFetcher ??= new SuccessStopAsyncHandlerChain<ApsInterstitialConfig>(IoCExtensions.Resolve<ApsVideoFetcher>(), IoCExtensions.Resolve<ApsStaticFetcher>());
#else
        private IAsyncHandler<ApsInterstitialConfig> ApsInterstitialFetcher => null;
#endif
        
        public bool IsInitialized { get; private set; }
        private IAsyncOperation _initOperation;
        private IAsyncOperation<TimeOut> _loadOperation;
        private IAsyncOperation _showOperation;
        
        private readonly List<LevelPlayInterstitialAdWrapper> _interstitialWrappers = new();
        private StateData _stateData;
        
        public IAsyncOperation Initialize()
        {
            if (_initOperation != null) return _initOperation;
            _initOperation = new SharedAsyncOperation().Success();
            IsInitialized = true;
            this.LogInfo();
            _SetupWrappers();
            _stateData = new StateData()
            {
                AdService = _adService,
                Wrappers = _interstitialWrappers,
                LoadState = new LoadState(),
                ShowState = new ShowState(),
                InterstitialAdRepository = _interstitialAdRepository,
            };
            return _initOperation;
        }

        private void _SetupWrappers()
        {
            _interstitialWrappers.DestroyAd();
            _interstitialWrappers.Clear();
            var c = _multipleInterstitialAdsConfigRepository.Get();
            if (c.Unlocked)
            {
                var i = 0;
                foreach (var adUnit in c.AdUnits)
                {
                    var wrapper = new LevelPlayInterstitialAdWrapper(i.ToString(), _config.InterstitialFakeLoadingTime)
                    {
                        UnitId = adUnit.Id,
                        PlacementName = adUnit.PlacementName,
                        ApsFetcher = ApsInterstitialFetcher,
                        ApsInterstitialConfig = adUnit.ToApsInterstitialConfig(),
                        AdsBreakConfigRepository = _adsBreakConfigRepository,
                    };
                    wrapper.Create();
                    _interstitialWrappers.Add(wrapper);
                    i++;
                }
            }
            else
            {
                var wrapper = new LevelPlayInterstitialAdWrapper("0", _config.InterstitialFakeLoadingTime)
                {
                    UnitId = _config.IronSourceInterstitialId,
                    PlacementName = null,
                    ApsFetcher = ApsInterstitialFetcher,
                    ApsInterstitialConfig = _config.ToApsInterstitialConfig(),
                    AdsBreakConfigRepository = _adsBreakConfigRepository,
                };
                wrapper.Create();
                _interstitialWrappers.Add(wrapper);
            }
            this.LogInfo(nameof(c.Unlocked), c.Unlocked, nameof(_interstitialWrappers.Count), _interstitialWrappers.Count);
        }

        /// ------------------------------------------------------------------------------------------------------------
        /// Auto Load Functions
        /// ------------------------------------------------------------------------------------------------------------
        public void Update()
        {
            if (_stateData?.CurrentState == null) return;
            _stateData.CurrentState = _stateData.CurrentState?.Update(_stateData);
        }
        
        public void OnApplicationPause(bool pauseStatus)
        {
            this.LogInfo(nameof(pauseStatus), pauseStatus);
            _stateData?.ShowWrapper?.OnApplicationPause(pauseStatus);
        }

        /// ------------------------------------------------------------------------------------------------------------
        /// Load Functions
        /// ------------------------------------------------------------------------------------------------------------
        public void StartLoadLoop()
        {
            _stateData.CurrentState = _stateData.LoadState;
        }
        /// ------------------------------------------------------------------------------------------------------------
        /// Show Functions
        /// ------------------------------------------------------------------------------------------------------------
        public IAsyncOperation Show(IAdPlacement adPlacement)
        {
            this.LogInfo(nameof(adPlacement), adPlacement);
            if (Application.isEditor) return new SharedAsyncOperation().Fail("Application.isEditor");
            if (_stateData.CurrentState == _stateData.ShowState) return _stateData.ShowOperation;
            if (PlacementValidator != null && !PlacementValidator.Validate(adPlacement.Name)) 
                return new SharedAsyncOperation().Fail($"!PlacementValidator.Validate({adPlacement.Name})");

            this.LogInfo("revMap", _interstitialWrappers.ToRevenueMap());
            
            LevelPlayInterstitialAdWrapper bestOne = null;
            for (var i = 0; i < _interstitialWrappers.Count; i++)
            {
                if (_interstitialWrappers[i].CachedAdInfo == null || !_interstitialWrappers[i].IsAdReady()) continue;
                if (bestOne == null)
                {
                    bestOne = _interstitialWrappers[i];
                    continue;
                }
                var oldRev = bestOne.CachedAdInfo.FixedRevenue();
                var newRev = _interstitialWrappers[i].CachedAdInfo.FixedRevenue();
                if (oldRev < newRev) bestOne = _interstitialWrappers[i];
            }

            if (bestOne == null)
            {   
                this.Track(AdNotReadyParams.Interstitial(adPlacement.Name));
                this.LogError(nameof(bestOne), "null");
                return new SharedAsyncOperation().Fail("bestOne == null");
            }
            this.LogInfo("bestRevenueId", bestOne.Id, "rev", bestOne.CachedAdInfo.FixedRevenue());   
            this.Track(AdReadyParams.Interstitial(adPlacement.Name));
            
            _stateData.ShowOperation = bestOne.ShowAd(adPlacement);
            _stateData.ShowWrapper = bestOne;
            _stateData.CurrentState = _stateData.ShowOperation != null ? _stateData.ShowState : _stateData.CurrentState;
            _stateData.InterstitialAdRepository = _interstitialAdRepository;
            return _stateData.ShowOperation;
        }

        public class StateData
        {
            public IAdService AdService { get; set; }
            public List<LevelPlayInterstitialAdWrapper> Wrappers { get; set; }
            public LoadState LoadState { get; set; }
            public ShowState ShowState { get; set; }
            
            public IState CurrentState { get; set; }
            public LevelPlayInterstitialAdWrapper ShowWrapper { get; set; }
            public IAsyncOperation ShowOperation { get; set; }
            public InterstitialAdRepository InterstitialAdRepository { get; set; }
        }
        
        public interface IState
        {
            IState Update(StateData stateData);
        }
        
        public class LoadState : IState
        {
            public IState Update(StateData stateData)
            {
                if (Time.frameCount % 120 != 0) return this;
                if (!SharedUtilities.IsInternetReachability()) return this;
                if (!stateData.AdService.IsInitialized) return this;
                
                foreach (var wrapper in stateData.Wrappers)
                {
                    if (wrapper.RequireCreate)
                    {
                        if (!wrapper.CouldCreate) continue; //New logic that limit retry times to prevent spam ad to the server.
                        wrapper.Create();
                    }
                    if (wrapper.RequireLoad) wrapper.LoadAd();
                    if (wrapper.LifeCycleState == LifeCycleState.LoadStart) continue;
                    if (wrapper.CachedAdInfo != null) continue;
                    // if (!wrapper.IsZeroRevenueAd) continue;
                    wrapper.Create();
                    wrapper.LoadAd();
                }
                return this;
            }
        }
        
        public class ShowState : IState
        {
            public IState Update(StateData stateData)
            {
                if (!stateData.ShowOperation.IsComplete) return this;
                stateData.ShowOperation = null;
                SharedNotificationCenter.Notify(NotificationId.InterstitialCompleted);
                if (stateData.InterstitialAdRepository != null)
                {
                    var e = stateData.InterstitialAdRepository.Get();
                    e.CloseTime = DateTime.Now;
                    stateData.InterstitialAdRepository.Save(e);
                }
                return stateData.LoadState;
            }
        }
    }
}
#endif