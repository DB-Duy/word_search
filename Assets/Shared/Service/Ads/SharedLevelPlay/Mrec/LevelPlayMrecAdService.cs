#if LEVEL_PLAY
using System.Collections;
using System.Collections.Generic;
using Shared.Core.Async;
using Shared.Core.IoC;
using Shared.Entity.Config;
using Shared.Repository.Ads;
using Shared.Service.Ads.Common;
using Shared.Service.SharedCoroutine;
using Shared.Utils;
using UnityEngine;
using Zenject;

namespace Shared.Service.Ads.SharedLevelPlay.Mrec
{
    [Service]
    public class LevelPlayMrecAdService : IMrecAd, Shared.Core.Handler.Corou.Initializable.IInitializable, ISharedUtility
    {
        [Inject] private IConfig _config;
        [Inject] private MrecPositionRepository _mrecPositionRepository;
        
        [Inject(Optional = true)] private IApsMrecFetcher _apsMrecFetcher;
        
        private readonly Dictionary<string, LevelPlayMrecWrapper> _mrecWrappers = new();
        private readonly List<LevelPlayMrecWrapper> _mrecWrappersList = new();
        
        private IAsyncOperation _initOperation;
        public bool IsInitialized { get; private set; }
        public IAsyncOperation Initialize()
        {
            if (_initOperation != null) return _initOperation;
            _initOperation = new SharedAsyncOperation().Success();
            IsInitialized = true;

            this.StartSharedCoroutine(_WaitAndInitialize());
            
            return _initOperation;
        }

        private IEnumerator _WaitAndInitialize()
        {
            while (!AdFlag.IsInitialized) yield return null;
            // var e = _mrecPositionRepository.Get();
            // if (e != null && !e.IsEmpty())
            // {
            //     foreach (var ee in e.Positions)
            //     {
            //         var wrapper = new LevelPlayMrecWrapper(ee.Key, e.GetPosition(ee.Key), _config.IronSourceMrecId, ee.Key)
            //         {
            //             ApsFetcher = _apsMrecFetcher,
            //             ApsSlotId = _config.ApsMrec
            //         };
            //         wrapper.CreateAd();
            //         wrapper.LoadAd();
            //         _mrecWrappers.Add(ee.Key, wrapper);
            //         _mrecWrappersList.Add(wrapper);
            //     }
            // }
            
            AdFlag.IsMrecInitialized = true;
            
        }

        public void Show(IAdPlacement placement)
        {
            var wrapper = _mrecWrappers.GetValueOrDefault(placement.Name);
            if (wrapper == null)
            {
                this.LogError(SharedLogTag.AdNLevelPlayNMrec, nameof(placement), placement.Name, nameof(wrapper), "null");
                return;
            }
            wrapper.ShowAd();
        }

        public void Hide(IAdPlacement placement)
        {
            var wrapper = _mrecWrappers.GetValueOrDefault(placement.Name);
            if (wrapper == null)
            {
                this.LogError(SharedLogTag.AdNLevelPlayNMrec, nameof(placement), placement.Name, nameof(wrapper), "null");
                return;
            }
            wrapper.HideAd();
        }

        public void Register(IAdPlacement placement, Vector2 position)
        {
            var wrapper = _mrecWrappers.GetValueOrDefault(placement.Name);
            if (wrapper == null)
            {
                wrapper = new LevelPlayMrecWrapper(placement.Name, position, _config.IronSourceMrecId, placement.Name)
                {
                    ApsFetcher = _apsMrecFetcher,
                    ApsSlotId = _config.ApsMrec
                };
                wrapper.CreateAd();
                wrapper.LoadAd();
                _mrecWrappers.Add(placement.Name, wrapper);
                _mrecWrappersList.Add(wrapper);
                _mrecPositionRepository.Add(placement.Name, position);
            }
            else
            {
                var result = wrapper.CreateIfPositionChanged(position);
                if (result)
                {
                    wrapper.LoadAd();
                    _mrecPositionRepository.Add(placement.Name, position);
                }
            }
        }
    }
}
#endif