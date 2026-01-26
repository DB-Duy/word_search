#if LEVEL_PLAY && !UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using com.unity3d.mediation;
using Shared.Core.Async;
using Shared.Core.IoC;
using Shared.Core.IoC.UnityLifeCycle;
using Shared.Core.Validator;
using Shared.Entity.Config;
using Shared.Repository.Ads;
using Shared.Service.Ads.Validator;
using Shared.Service.Aps;
using Shared.Service.SharedCoroutine;
using Shared.Utilities;
using Shared.Utils;
using UnityEngine;
using Zenject;

namespace Shared.Service.Ads.SharedLevelPlay.Banner
{
    /// <summary>
    /// https://developers.is.com/ironsource-mobile/unity/banner-integration-unity/#step-5
    /// </summary>
    [Service]
    public class LevelPlayBannerAdService : IBannerAd, ISharedUtility, IUnityUpdate, ISharedLogTag
    {
        public string LogTag => SharedLogTag.AdNLevelPlayNBanner_;

        [Inject] private IAdService _adService;
        [Inject] private MultipleBannerAdsConfigRepository _multipleBannerAdsConfigRepository;
        [Inject] private IConfig _config;

        private IApsBannerFetcher _apsBannerFetcher;
        private IApsBannerFetcher ApsBannerFetcher => _apsBannerFetcher ??= IoCExtensions.TryResolve<IApsBannerFetcher>();
        
        private IValidator _validator;
        private IValidator Validator => _validator ??= ValidatorChain.CreateChainFromType<IBannerValidator>();
        
        private IAsyncOperation _loadOperation;

        private IAsyncOperation _initOperation;
        public bool IsInitialized { get; private set; }

        private readonly List<LevelPlayBannerWrapper> _bannerWrappers = new();
        private readonly StateData _stateData = new();
        
        public IAsyncOperation Initialize()
        {
            if (_initOperation != null) return _initOperation;
            _initOperation = new SharedAsyncOperation().Success();
            IsInitialized = true;
            
            _stateData.BannerWrappers = _bannerWrappers;
            _stateData.FindState = new FindState();
            _stateData.MonitorDisplayedAdState = new MonitorDisplayedAdState();

            IronSourceEvents.onImpressionDataReadyEvent += _onImpressionDataReadyEvent;
            
            return _initOperation;
        }
        
        public bool Validate() => Validator == null || Validator.Validate();

        public void LoadBanner()
        {
            if (_loadOperation != null) return;
            if (Validator != null && !Validator.Validate()) return;
            DestroyBanner();
            this.StartSharedCoroutine(_LoadBanner());
        }
        
        private IEnumerator _LoadBanner()
        {
            _loadOperation = new SharedAsyncOperation();
#if LOG_INFO
            if (!_adService.IsInitialized) this.LogWarning(nameof(_adService.IsInitialized), _adService.IsInitialized, "action", "wait");
#endif
            while (!_adService.IsInitialized) yield return null;
            
            var entity = _multipleBannerAdsConfigRepository.Get();
            if (entity.Unlocked)
            {
                for (var i = 0; i < entity.AdUnits.Count; i++)
                {
                    var adUnit = entity.AdUnits[i];
                    var ins = new LevelPlayBannerWrapper(i)
                    {
                        AdUnitId = adUnit.Id,
                        AdSize = LevelPlayAdSize.CreateAdaptiveAdSize(),
                        Position = LevelPlayBannerPosition.BottomCenter,
                        PlacementName = adUnit.PlacementName,
                        DisplayOnLoad = false,
                        
                        ApsBannerFetcher = ApsBannerFetcher,
                        ApsBannerConfig = adUnit.ToApsBannerConfig()
                    };
                    ins.CreateBannerAd();
                    ins.PauseAutoRefresh();
                    _bannerWrappers.Add(ins);
                }
                _stateData.RefreshRateInSeconds = entity.RefreshRateInSeconds;
                _stateData.CurrentState = _stateData.FindState;
            }
            else
            {
                var i = new LevelPlayBannerWrapper(0)
                {
                    AdUnitId = _config.IronSourceBannerId,
                    AdSize = LevelPlayAdSize.CreateAdaptiveAdSize(),
                    Position = LevelPlayBannerPosition.BottomCenter,
                    PlacementName = null,
                    DisplayOnLoad = true,
                        
                    ApsBannerFetcher = ApsBannerFetcher,
                    ApsBannerConfig = _config.ToApsBannerConfig()
                };
                i.CreateBannerAd();
                _bannerWrappers.Add(i);
#if LOG_INFO
                if (!_adService.IsInitialized) this.LogWarning(nameof(_adService.IsInitialized), _adService.IsInitialized, "action", "wait");
#endif
                while (!_adService.IsInitialized) yield return null;
                _bannerWrappers.LoadBannerAd();
                _stateData.CurrentState = null;
            }

            this.LogInfo(nameof(entity.Unlocked), entity.Unlocked, nameof(_bannerWrappers), _bannerWrappers.Count);
            yield return null;
        }

        public void DestroyBanner()
        {
            if (!IsInitialized) return;
            this.LogInfo();
            _bannerWrappers.DestroyBannerAd();
            _bannerWrappers.Clear();
            _stateData.CurrentState = null;
            _loadOperation = null;
        }

        private void _onImpressionDataReadyEvent(IronSourceImpressionData data)
        {
            if (!data.adFormat.Equals("banner")) return;
            _bannerWrappers.OnBannerImpressionDataReadyEvent(data);
        }

        public void Update()
        {
            if (_stateData?.CurrentState == null) return;
            _stateData.CurrentState = _stateData.CurrentState?.Update(_stateData);
        }
        
        public class StateData : ISharedLogTag
        {
            public string LogTag => SharedLogTag.AdNLevelPlayNBanner_;
            
            public float RefreshRateInSeconds { get; set; }
            public List<LevelPlayBannerWrapper> BannerWrappers { get; set; }
            public FindState FindState { get; set; }
            public MonitorDisplayedAdState MonitorDisplayedAdState { get; set; }
            
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
            
            public LevelPlayBannerWrapper DisplayedBanner { get; set; }
            public float DisplayTtl { get; set; }
        }
        
        public interface IState
        {
            IState Update(StateData stateData);
        }
        
        public class FindState : IState, ISharedUtility
        {
            public IState Update(StateData stateData)
            {
                if (Time.frameCount % 90 != 0) return this;
                if (!SharedUtilities.IsInternetReachability()) return this;

                foreach (var i in stateData.BannerWrappers)
                {
                    if (i.RequireCreate) i.CreateBannerAd();
                    if (i.RequireLoad) i.LoadBannerAd();
                    if (!i.IsZeroRevenue) continue;
                    i.CreateBannerAd();
                    i.LoadBannerAd();
                }

                stateData.DisplayedBanner = stateData.BannerWrappers.ShowLargestRevenueBannerAd();
                if (stateData.DisplayedBanner == null) return this;
                stateData.DisplayTtl = stateData.RefreshRateInSeconds;
                return stateData.MonitorDisplayedAdState;
            }
        }
        
        public class MonitorDisplayedAdState : IState
        {
            public IState Update(StateData stateData)
            {
                if (!stateData.DisplayedBanner.IsDisplaying) return stateData.FindState;
                stateData.DisplayTtl -= Time.unscaledDeltaTime;
                return stateData.DisplayTtl > 0 ? this : stateData.FindState;
            }
        }
    }
}
#endif