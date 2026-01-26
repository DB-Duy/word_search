#if LEVEL_PLAY && !UNITY_EDITOR
using System.Collections.Generic;
using System.Linq;
using Shared.Core.Async;
using Shared.Core.Handler.Async;
using Shared.Core.IoC;
using Shared.Core.IoC.UnityLifeCycle;
using Shared.Entity.Config;
using Shared.Repository.Ads;
using Shared.Service.Ads.Common;
using Shared.Service.NotificationCenter;
using Shared.Service.Tracking;
using Shared.Tracking.Models.Ads;
using Shared.Utilities;
using Shared.Utils;
using UnityEngine;
using Zenject;

namespace Shared.Service.Ads.SharedLevelPlay.RV
{
    [Service]
    public class LevelPlayRewardAdService : IUnityUpdate, IRewardAd, ISharedUtility, ISharedLogTag, IUnityOnApplicationPause
    {
        public string LogTag => SharedLogTag.AdNLevelPlayNReward_;

        [Inject] private IAdService _adService;
        [Inject] private IConfig _config;
        [Inject] private MultipleRewardedAdsConfigRepository _multipleRewardedAdsConfigRepository;

        [Inject(Optional = true)] private IRewardedAdFailMessageResolver _failMessageResolver;
        private IRewardedAdFailMessageResolver FailMessageResolver => _failMessageResolver ??= new RewardedAdFailMessageResolver();

        [Inject(Optional = true)] private IApsRewardFetcher _apsRewardFetcher;
        private IAsyncHandler<string> ApsRewardedFetcher => _apsRewardFetcher as IAsyncHandler<string>;
        
        private readonly List<LevelPlayRewardAdWrapper> _rewardWrappers = new();
        private bool _isRunning = false;

        public bool IsInitialized { get; private set; }
        private IAsyncOperation _initOperation;
        
        private StateData _stateData;
        /// ------------------------------------------------------------------------------------------------------------
        /// Setup Functions
        /// ------------------------------------------------------------------------------------------------------------
        
        public IAsyncOperation Initialize()
        {
            if (_initOperation != null) return _initOperation;
            this.LogInfo();
            _stateData = new StateData()
            {
                AdService = _adService,
                Wrappers = _rewardWrappers,
                LoadState = new LoadState(),
                ShowState = new ShowState(),
                CurrentState = null,
                DisplayedWrapper = null,
                ShowOperation = null,
            };
            _CreateWrappers();
            IsInitialized = true;
            _initOperation = new SharedAsyncOperation().Success();
            return _initOperation;
        }

        private void _CreateWrappers()
        {
            this.LogInfo();
            _rewardWrappers.DestroyAd();
            _rewardWrappers.Clear();
            var c = _multipleRewardedAdsConfigRepository.Get();
            if (c.Unlocked)
            {
                var i = 0;
                foreach (var adUnit in c.AdUnits)
                {
                    var wrapper = new LevelPlayRewardAdWrapper(i.ToString())
                    {
                        AdUnitId = adUnit.Id,
                        PlacementName = adUnit.PlacementName,
                        FailMessageResolver = FailMessageResolver,
                        ApsRewardedFetcher = ApsRewardedFetcher,
                        ApsSlotId = adUnit.ApsId
                    };
                    _rewardWrappers.Add(wrapper);
                    i++;
                }
            }
            else
            {
                var wrapper = new LevelPlayRewardAdWrapper("0")
                {
                    AdUnitId = _config.IronSourceRewardedId,
                    PlacementName = null,
                    FailMessageResolver = FailMessageResolver,
                    ApsRewardedFetcher = ApsRewardedFetcher,
                    ApsSlotId = _config.ApsRewardedVideo
                };
                _rewardWrappers.Add(wrapper);
            }
            _rewardWrappers.CreateAd();
        }

        /// ------------------------------------------------------------------------------------------------------------
        /// Load Functions
        /// ------------------------------------------------------------------------------------------------------------
        public void StartLoadLoop()
        {
            this.LogInfo(nameof(_rewardWrappers.Count), _rewardWrappers.Count);
            _isRunning = true;
            _stateData.CurrentState = _stateData.LoadState;
        }
        
        /// ------------------------------------------------------------------------------------------------------------
        /// Show Functions
        /// ------------------------------------------------------------------------------------------------------------
        public IAsyncOperation<RewardedAdOperation> Show(IAdPlacement placement)
        {
#if UNITY_EDITOR
            if (Application.isEditor) return new SharedAsyncOperation<RewardedAdOperation>(null).Fail("Application.isEditor");
#endif
            if (_stateData.ShowOperation != null) return _stateData.ShowOperation;
            
            this.LogInfo(nameof(placement), placement, "revMap", _rewardWrappers.ToRevenueMap());

            LevelPlayRewardAdWrapper bestOne = null;
            foreach (var wrapper in _rewardWrappers.Where(wrapper => wrapper.IsAdReady() && wrapper.CachedAdInfo != null))
            {
                if (bestOne == null)
                {
                    bestOne = wrapper;
                    continue;
                }
                
                var br = bestOne.CachedAdInfo.FixedRevenue();
                var r = wrapper.CachedAdInfo.FixedRevenue();
                if (br < r) bestOne = wrapper;
            }

            if (bestOne == null)
            {
                this.Track(AdNotReadyParams.Rewarded(placement.Name));
                this.LogInfo(nameof(bestOne), "null");
                return new SharedAsyncOperation<RewardedAdOperation>(new RewardedAdOperation(placement.Name, FailMessageResolver.ResolveUserMessage())).Fail("bestOne == null");
            }

            this.LogInfo(nameof(bestOne.Id), bestOne.Id, nameof(bestOne.CachedAdInfo), bestOne.CachedAdInfo.DebugLessField());
            _stateData.DisplayedWrapper = bestOne;
            _stateData.ShowOperation = bestOne.ShowAd(placement);
            _stateData.CurrentState = _stateData.ShowState;
            return _stateData.ShowOperation;
        }
        
        // -------------------------------------------------------------------------------------------------------------
        // Inner functions
        // -------------------------------------------------------------------------------------------------------------
        public void Update()
        {
            if (!_isRunning) return;
            if (_stateData == null) return;
            _stateData.CurrentState = _stateData.CurrentState?.Update(_stateData);
        }
        
        public void OnApplicationPause(bool pauseStatus)
        {
            this.LogInfo(nameof(pauseStatus), pauseStatus);
            _stateData?.DisplayedWrapper?.OnApplicationPause(pauseStatus);
        }

        // -------------------------------------------------------------------------------------------------------------
        // State Definitions
        // -------------------------------------------------------------------------------------------------------------
        public class StateData
        {
            public IAdService AdService { get; set; }
            public List<LevelPlayRewardAdWrapper> Wrappers { get; set; }
            public LoadState LoadState { get; set; }
            public ShowState ShowState { get; set; }
            private IState _currentState;
            public IState CurrentState { 
                get => _currentState;
                set
                {
                    if (value == _currentState) return;
                    this.LogInfo(nameof(CurrentState), _currentState?.GetType().Name, "newState", value?.GetType().Name);
                    _currentState = value;
                }
            }
            
            public LevelPlayRewardAdWrapper DisplayedWrapper { get; set; }
            public IAsyncOperation<RewardedAdOperation> ShowOperation { get; set; }    
        }
        
        public interface IState
        {
            IState Update(StateData stateData);
        }
        
        public class LoadState : IState
        {
            public IState Update(StateData stateData)
            {
                if (Time.frameCount % 150 != 0) return this;
                if (!stateData.AdService.IsInitialized) return this;
                if (!SharedUtilities.IsInternetReachability()) return this;

                foreach (var ad in stateData.Wrappers)
                {
                    if (ad.RequireCreate)
                    {
                        if (!ad.CouldCreate) continue; //New logic that limit retry times to prevent spam ad to the server.
                        ad.Create();
                    }
                    if (ad.RequireLoad) ad.LoadAd();
                    if (ad.LifeCycleState == LifeCycleState.LoadStarted) continue;
                    if (ad.CachedAdInfo != null) continue;
                    // if (!ad.ContainsZeroRevenue()) continue;
                    ad.Create();
                    ad.LoadAd();
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
                stateData.DisplayedWrapper = null;
                SharedNotificationCenter.Notify(NotificationId.RewardedCompleted);
                return stateData.LoadState;
            }
        }
    }
}
#endif