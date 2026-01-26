using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Shared.Core.IoC;
using Shared.Repository.InPlayAds;
using Shared.Service.InPlayAds;
using Shared.Utils;
using Shared.View.Adverty5;
using Shared.View.Gadsme;
using UnityEngine;
using Zenject;

namespace Shared.View.InPlayAds
{
    /// <summary>
    /// 1 placement sẽ chứa các ads của nó, nó tự mediation và pick ra thằng tốt nhất để show.
    /// </summary>
    [DisallowMultipleComponent]
    public class UIInPlayAdPlacementV2 : IoCMonoBehavior, ISharedUtility
    {
        private const int MODE_NOT_INIT = -1;
        private const int MODE_MEDIATION = 0;
        private const int MODE_SHOW = 1;
        private const int MODE_REMOVE_ADS = 2;
        private const int MODE_REMOVE_ADS_BY_OVERLAP = 3;
        
        private const float TIME_MEDIATION = 3;
        private const float TIME_SHOW = 20;
        private const float TIME_REMOVE_ADS = 10;
        
        [SerializeField] private string placementName;
        [SerializeField] private AbstractInPlayAd[] ads;
        [SerializeField] private Vector3 hiddenOffset = new(0, 1000, 0);
        [SerializeField] private RectTransform[] overlapChecks; // [ Inject]
        
        public string PlacementName => placementName;

        [Inject] private InPlayAdsConfigRepository _configRepository;
        [Inject(Optional = true)] private IInPlayAdService _service;
        
        private float _timer;
        private int _mode = MODE_NOT_INIT;
        public AbstractInPlayAd CurrentAd { get; private set; }

        private void Start()
        {
#if LOG_INFO            
            for (var i = 0; i < ads.Length; i++)
                if (ads[i] == null)
                    this.LogError("error", $"ads[{i}] == null");
#endif
            if (_service == null) gameObject.SetActive(false);
            InPlayAdRegistry.Register(this);
            this.LogInfo(SharedLogTag.AudioAds, "name", name);
            ads.SetLocalPosition(hiddenOffset);
            StartCoroutine(_ValidateByOverlap());
#if LOG_INFO
            gameObject.GetComponent<RectTransform>().AddRedImage();
#else
            gameObject.GetComponent<RectTransform>().RemoveRedImage();
#endif
        }
        
        private IEnumerator _ValidateByOverlap()
        {
            // Delay 2 frames
            yield return null;
            yield return null;
            
            var me = GetComponent<RectTransform>();
            var overlap = me.IsOnScreenOverlapOneOf(overlapChecks);
            ads.SetActive(!overlap);
            _mode = overlap ? MODE_REMOVE_ADS_BY_OVERLAP : MODE_MEDIATION;
            if (!overlap) yield break;
            gameObject.GetComponent<RectTransform>().RemoveRedImage();
            _HideAd();
            this.LogInfo(SharedLogTag.InPlayAds, "des", "Overlap!!! ads.SetActive(false);");
        }

        private void OnDestroy()
        {
            InPlayAdRegistry.Remove(this);
        }

        private void Update()
        {
            if (_mode is MODE_NOT_INIT or MODE_REMOVE_ADS_BY_OVERLAP) return;
            if (ads.Length == 0) return;
            _timer += Time.unscaledDeltaTime;
            if (_mode == MODE_REMOVE_ADS)
            {
                if (_timer > TIME_REMOVE_ADS)
                {
                    _timer = 0;
                    var result = _service.ValidateByValidators(placementName);
                    if (result)
                    {
                        _mode = MODE_MEDIATION;
                        ads.ActiveByProvider(InPlayAdRegistry.Provider);
                    }
                    else _mode = MODE_REMOVE_ADS;
                }
            }
            else if (_mode == MODE_MEDIATION)
            {
                if (_timer > TIME_MEDIATION)
                {
                    if (_ShowAd())
                    {
                        _timer = 0;
                        _mode = MODE_SHOW;
                    }
                    else
                    {
                        _timer = 0;
                    }
                }
            }
            else if (_mode == MODE_SHOW)
            {
                if (_timer > TIME_SHOW)
                {
                    _timer = 0;
                    _mode = MODE_MEDIATION;
                }
            }
        }
        
        private bool _ShowAd()
        {
            var readyAds = ads.Where(ad => ad.IsReady).ToList();
            if (readyAds.Count == 0)
            {
                this.LogInfo(SharedLogTag.InPlayAds, nameof(readyAds.Count), readyAds.Count);
                return false;
            }
            var config = _configRepository.Get(); // update config
            if (config.Placements == null)
            {
                this.LogError(SharedLogTag.InPlayAds, "exit", "config.Placements == null");
                return false;
            }
            if (config.Placements.Count == 0)
            {
                this.LogError(SharedLogTag.InPlayAds, "exit", "config.Placements.Count == 0");
                return false;
            }
            if (!config.Placements.ContainsKey(placementName))
            {
                this.LogError(SharedLogTag.InPlayAds, "exit", $"!config.Placements.ContainsKey({placementName})");
                return false;
            }
            this.LogInfo(SharedLogTag.InPlayAds, "readyAds", readyAds.Count);
            var dict = readyAds.ToDictionary(ad => ad.GetType().Name);
            var classNames = new List<string>(dict.Keys);
            var placementConfig = config.Placements.Get(placementName, null);
            classNames.Sort((a, b) => placementConfig.IndexOf(a).CompareTo(placementConfig.IndexOf(b)));
            var newAd = dict[classNames[0]];
            if (newAd != CurrentAd)
            {
                _HideAd();
                CurrentAd = newAd;
            }
            CurrentAd.transform.localPosition = Vector3.zero;
            CurrentAd.ResetReadyState();
            this.LogInfo(SharedLogTag.InPlayAds, nameof(CurrentAd), CurrentAd.GetType().Name, nameof(CurrentAd.transform.localPosition), CurrentAd.transform.localPosition.ToString(), nameof(classNames), classNames, nameof(placementConfig), placementConfig);
            return true;
        }

        private void _HideAd()
        {
            if (CurrentAd == null) return;
            this.LogInfo(SharedLogTag.InPlayAds, nameof(CurrentAd), CurrentAd.GetType().Name);
            CurrentAd.transform.localPosition = hiddenOffset;
            CurrentAd = null;
        }

        public void RemoveAds()
        {
            _HideAd();
            _mode = MODE_REMOVE_ADS;
            ads.SetActive(false);
        }
        
        
    }
    public static class InPlayAdUtils
    {
        public static void SetActive(this AbstractInPlayAd[] me, bool active)
        {
            foreach (var i in me)
                i.SetActive(active);
        }
        
        public static void ActiveByProvider(this AbstractInPlayAd[] me, InPlayAdProvider provider)
        {
            switch (provider)
            {
                case InPlayAdProvider.Adverty:
                {
                    foreach (var i in me)
                        i.SetActive(i is Adverty5InPlayAd);
                    break;
                }
                case InPlayAdProvider.Gadsme:
                {
                    foreach (var i in me)
                        i.SetActive(i is GadsmeInPlayAd);
                    break;
                }
                default:
                {
                    foreach (var i in me)
                        i.SetActive(true);
                    break;
                }
            }
        }
        
        public static void SetLocalPosition(this AbstractInPlayAd[] me, Vector3 localPosition)
        {
            foreach (var i in me)
                i.MyTransform.localPosition = localPosition;
        }
    }
}