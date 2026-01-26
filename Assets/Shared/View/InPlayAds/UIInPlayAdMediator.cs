using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Shared.Core.IoC;
using Shared.Repository.InPlayAds;
using Shared.Service.InPlayAds;
using Shared.Utilities;
using Shared.Utils;
using UnityEngine;
using Zenject;

namespace Shared.View.InPlayAds
{
    [DisallowMultipleComponent]
    public class UIInPlayAdMediator : IoCMonoBehavior
    {
        private const float TimeIdle = 30f;
        
        private const int StateMediation = 1;
        private const int StateIdle = 2;
        
        [SerializeField] private string placementName;
        public string PlacementName => placementName;
        
        [Inject] private InPlayAdsConfigRepository _configRepository;
        [Inject(Optional = true)] private IInPlayAdService _service;
        [Inject(Optional = true)] private IInPlayAdAdapter _adapter;
        
        private List<string> _priorityList;
        private readonly Dictionary<string, AbstractInPlayAd> _adDict = new();
        private int _updatePriorityListFrameCount = 1;
        private bool _isEnabled = false;
        private string _currentAdClassName;
        private AbstractInPlayAd _currentAd;
        private int _currentState = StateMediation;
        private float _timeCount;

        private void OnEnable()
        {
            InPlayAdRegistry.Register(this);
        }

        private void OnDisable()
        {
            InPlayAdRegistry.Remove(this);
        }
        
        private void Update()
        {
            if (_service == null) return;
            if (!_service.IsInitialized) return;
            
            if (Time.frameCount % _updatePriorityListFrameCount == 0)
            {
                _updatePriorityListFrameCount = 300;
                _UpdatePriorityList();
            }

            if (!_service.ValidateByValidators(placementName))
            {
                _RemoveCurrentAdIfExisted();
                return;
            }

            if (!_isEnabled) return;
            _timeCount -= Time.deltaTime;
            switch (_currentState)
            {
                case StateIdle:
                {
                    if (_timeCount <= 0)
                        _currentState = StateMediation;

                    if (_currentAd != null)
                    {
                        var p = InPlayAdRegistry.GetPlacement(this);
                        if (p == null) return;
                        if (p.IsOverLapOtherUis is null or true) return;
                        if (!_currentAd.IsReady) return;
                        _MoveAdToPlacement(_currentAd, p);
                    }
                    break;
                }
                case StateMediation:
                {
                    _Mediate();
                    _timeCount = TimeIdle;
                    _currentState = StateIdle;
                    this.LogInfo(SharedLogTag.InPlayAds, "f", "Mediate", nameof(placementName), placementName, nameof(_currentAd.ClassName), _currentAd?.ClassName, "IsReady", _currentAd?.IsReady, nameof(_priorityList), _priorityList?.ToJsonString());
                    break;
                }
            }
        }

        private void _UpdatePriorityList()
        {
            // this.LogInfo(SharedLogTag.InPlayAds);
            if (_configRepository == null)
            {
                this.LogInfo(SharedLogTag.InPlayAds, nameof(placementName), placementName, "ignore", "_configRepository == null");
                return;
            }

            var c = _configRepository.Get();
            if (c == null) 
            {
                this.LogInfo(SharedLogTag.InPlayAds, nameof(placementName), placementName, "ignore", "c == null");
                return;
            }
            _isEnabled = c.Unlocked;
            var p = c.Placements;
#if UNITY_EDITOR
            if (p == null)
            {
                p = new Dictionary<string, List<string>>()
                {
                    { "game_play", new List<string> { "GadsmeCanvasLeaderboard728x90", "GadsmeCanvasMobileLeaderboard320x50", "UIAdvertyFive320x50"}},
                    { "game_win", new List<string> { "GadsmeCanvasVideo4x3", "GadsmeCanvasVideo16x9", "GadsmeCanvasMediumRectangle300x250", "GadsmeCanvasBillboard970x250", "GadsmeCanvasLeaderboard728x90", "GadsmeCanvasMobileLeaderboard320x50", "GadsmeCanvasBannerSquare320x320", "UIAdvertyFive320x50", "UIAdvertyFive510x600"}},
                };
            }
#endif
            if (p == null)  
            {
                this.LogInfo(SharedLogTag.InPlayAds, "ignore", "p == null");
                return;
            }
            _priorityList = p.Get(placementName, null);

            if (_priorityList == null || _priorityList.IsEmpty()) 
            {
                this.LogInfo(SharedLogTag.InPlayAds, nameof(placementName), placementName, "ignore", "_priorityList == null || _priorityList.IsEmpty()");
                return;
            }
            _ModifyPriorityListByProvider();
            
            if (_priorityList.IsEmpty()) 
            {
                this.LogInfo(SharedLogTag.InPlayAds, nameof(placementName), placementName, "ignore", "_priorityList.IsEmpty() after _ModifyPriorityListByProvider()");
                return;
            }
            _CreateAds(_priorityList);
        }

        private void _CreateAds(List<string> classNames)
        {
            // this.LogInfo(SharedLogTag.InPlayAds, nameof(classNames), classNames.ToJsonString());
            classNames?.ForEach(className => StartCoroutine(_CreateAd(className)));
        }

        private IEnumerator _CreateAd(string className)
        {
            yield return null;
            if (_adDict.ContainsKey(className))
            {
                // this.LogInfo("f", "_CreateAd", "ignore", $"_adDict.ContainsKey({className})");
                yield break;
            }

            var path = string.Empty;
            if (className.Contains("Gadsme"))
                path = $"Shared/Gadsme/{className}";
            else if (className.Contains("AdvertyFive")) 
                path = $"Shared/Adverty5/{className}";
            else if (className.Contains("Adverty"))
                path = $"Shared/Adverty/{className}";

            if (_adapter != null)
            {
                var customPath = _adapter.GetPrefabPath(PlacementName, className);
                if (!string.IsNullOrEmpty(customPath))
                    path = customPath;
            }

            if (string.IsNullOrEmpty(path))
            {
                this.LogError(SharedLogTag.InPlayAds, nameof(placementName), placementName, "f", "_CreateAd", "ignore", $"string.IsNullOrEmpty(path) for {className}");
                yield break;
            }

            // Assets/Shared/Resources/Shared/Gadsme/GadsmeCanvasMediumRectangle300x250.prefab
            // Assets/Shared/Resources/Adverty5/UIAdvertyFive320x50.prefab
            _adDict.Add(className, null);
            var req = Resources.LoadAsync<GameObject>(path);
            yield return req;
            _adDict.Remove(className);
            if (req.asset == null)
            {
                this.LogError(SharedLogTag.InPlayAds, nameof(placementName), placementName, "f", "_CreateAd", "ignore", $"req.asset == null for {path}");
                yield break;
            }

            if (_adDict.ContainsKey(className))
            {
                this.LogInfo(SharedLogTag.InPlayAds, nameof(placementName), placementName, "f", "_CreateAd", "ignore", $"_adDict.ContainsKey({className})");
                yield break;
            }
            
            var go = Instantiate((GameObject)req.asset);
            go.name = className;
            var ad = go.GetComponent<AbstractInPlayAd>();
            if (ad == null)
            {
                this.LogError(SharedLogTag.InPlayAds, nameof(placementName), placementName, "f", "_CreateAd", "ignore", $"ad == null for {path}");
                yield break;
            }
            ad.ForPlacementName = placementName;
            ad.MyTransform.gameObject.SetActive(false);
            _adDict.Add(className, ad);
            _MoveAdBackToMediator(ad, false);
            this.LogInfo(SharedLogTag.InPlayAds, nameof(placementName), placementName, "f", "_CreateAd", "ad", className);
        }

        private void _SetUpCurrentAd(string className)
        {
            if (_currentAd != null) _MoveAdBackToMediator(_currentAd, false);

            _currentAdClassName = className;
            _currentAd = _adDict[className];
            _MoveAdBackToMediator(_currentAd, true);
        }

        private void _RemoveCurrentAdIfExisted()
        {
            if (_currentAd == null) return;
            _MoveAdBackToMediator(_currentAd, false);
            _currentAd = null;
        }

        private void _Mediate()
        {
            _ModifyPriorityListByProvider();
            if (_priorityList == null || _priorityList.IsEmpty())
            {
                // List empty thì remove current ad luôn.
                _RemoveCurrentAdIfExisted();
                return;
            }

            if (_currentAd != null && _currentAd.IsReady)
            {
                // Giữ nguyên, không thay đổi.
                this.LogInfo(SharedLogTag.InPlayAds, nameof(placementName), placementName, "note", "Keep current ad", nameof(_currentAd.ClassName), _currentAd.ClassName);
                return;
            }

            var lastIndex = string.IsNullOrEmpty(_currentAdClassName) ? -1 : _priorityList.IndexOf(_currentAdClassName);
            this.LogInfo(SharedLogTag.InPlayAds, "f", "Mediate", nameof(placementName), placementName, nameof(_currentAdClassName), _currentAdClassName, nameof(lastIndex), lastIndex, nameof(_currentAd.IsReady), _currentAd?.IsReady);
            // Chuyển qua mẫu ad mới. Remove mẫu cũ nếu có.
            _currentAdClassName = null;
            _RemoveCurrentAdIfExisted();

            if (lastIndex < 0)
            {
                // Scan từ đầu đến cuối list.
                foreach (var className in _priorityList)
                {
                    if (_adDict.ContainsKey(className) && _adDict[className] != null)
                    {
                        this.LogInfo(SharedLogTag.InPlayAds, "f", "Mediate", nameof(placementName), placementName, "note", "Scan from start to end.");
                        _SetUpCurrentAd(className);
                        break;
                    }
                }
                return;
            }

            // Tìm tiến lên từ lastIndex.
            for (var i = lastIndex + 1; i < _priorityList.Count; i++)
            {
                var className = _priorityList[i];
                if (_adDict.ContainsKey(className) && _adDict[className] != null)
                {
                    this.LogInfo(SharedLogTag.InPlayAds, "f", "Mediate", nameof(placementName), placementName, "note", $"Scan from {lastIndex + 1} to end.");
                    _SetUpCurrentAd(className);
                    break;
                }
            }
            // Nếu tìm thấy thì return
            if (!string.IsNullOrEmpty(_currentAdClassName)) return;

            for (var i = 0; i < lastIndex; i++)
            {
                var className = _priorityList[i];
                if (_adDict.ContainsKey(className) && _adDict[className] != null)
                {
                    this.LogInfo(SharedLogTag.InPlayAds, "f", "Mediate", nameof(placementName), placementName, "note", $"Scan from 0 to {lastIndex}.");
                    _SetUpCurrentAd(className);
                    break;
                }
            }
        }

        private void _MoveAdBackToMediator(AbstractInPlayAd ad, bool active)
        {
            if (ad == null) return;
            var isUI = _ValidateUIAd(ad.name);
            if (isUI)
            {
                var adTransform = ad.MyTransform;
                adTransform.SetParent(transform, false);
                adTransform.localPosition = Vector3.zero;
                adTransform.localScale = Vector3.one;
                adTransform.gameObject.SetActive(active);
            }
            else
            {
                var adTransform = ad.MyTransform;
                adTransform.position = transform.position;
                adTransform.localScale = Vector3.one;
                adTransform.gameObject.SetActive(active);
            }
        }

        private bool _ValidateUIAd(string adClassName)
        {
            return adClassName.Contains("UI") || adClassName.Contains("Canvas") || adClassName.Contains("InMenu");
        }

        private void _MoveAdToPlacement(AbstractInPlayAd ad, UIInPlayAdPlacement p)
        {
            if (ad == null)
            {
                this.LogError(SharedLogTag.InPlayAds, nameof(placementName), placementName, "f", nameof(_MoveAdToPlacement), "ignore", "ad == null");
                return;
            }

            if (p == null)
            {
                this.LogError(SharedLogTag.InPlayAds, nameof(placementName), placementName, "f", nameof(_MoveAdToPlacement), "ignore", "p == null");
                return;
            }
            if (!p.gameObject.activeSelf) return;
            if (ad.MyTransform.parent == p.transform) return;

            var isUI = _ValidateUIAd(ad.ClassName);
            
            if (isUI)
            {
                this.LogInfo(SharedLogTag.InPlayAds, nameof(placementName), placementName, "f", nameof(_MoveAdToPlacement), nameof(ad), ad.name, "p", p.name, nameof(isUI), isUI);
                var adTransform = (RectTransform)ad.MyTransform;
                var pTransform = (RectTransform)p.transform;
                adTransform.SetParent(pTransform, false);
                adTransform.localPosition = Vector3.zero;
                adTransform.localScale = Vector3.one;
                adTransform.gameObject.SetActive(true);
                adTransform.FullFillHeightByScale(pTransform);    
            }
            else
            {
                var adTransform = ad.MyTransform;
                var pTransform = p.transform;
                adTransform.position = new Vector3(pTransform.position.x, pTransform.position.y, adTransform.position.z);
                var spriteRenderer = adTransform.GetComponent<SpriteRenderer>();
                if (spriteRenderer != null)
                {
                    spriteRenderer.FullFillRectHeightByScale((RectTransform)pTransform);   
                }
                else
                {
                    var hasCustomScale = _adapter?.ContainsScale(placementName, ad.ClassName) ?? false;
                    if (hasCustomScale) adTransform.localScale = _adapter.GetScale(PlacementName, ad.ClassName);
                }
                
                adTransform.gameObject.SetActive(true);   
            }
        }

        public void RemoveAd()
        {
            this.LogInfo(SharedLogTag.InPlayAds);
            _RemoveCurrentAdIfExisted();
        }
        
        public void OnPlacementRemoved()
        {
            this.LogInfo(SharedLogTag.InPlayAds);
            _RemoveCurrentAdIfExisted();
        }

        private void _ModifyPriorityListByProvider()
        {
            if (_priorityList == null || _priorityList.IsEmpty()) return;
            _priorityList = InPlayAdRegistry.Provider switch
            {
                InPlayAdProvider.Adverty => _priorityList.FilterByContains("Adverty"),
                InPlayAdProvider.Gadsme => _priorityList.FilterByContains("Gadsme"),
                _ => _priorityList
            };
        }

#if UNITY_EDITOR
        private int _tryIndex = 0;
        public override void GUIEditor()
        {
            GUILayout.Label("Editor Quick Actions");

            if (GUILayout.Button("Try Stick To PlaceHolder"))
            {
                foreach (var ad in _adDict)
                {
                    ad.Value.MyTransform.SetParent(transform, false);
                    ad.Value.MyTransform.gameObject.SetActive(false);
                }
                _currentAd = _adDict[_priorityList[_tryIndex]];
                _currentAd.MyTransform.gameObject.SetActive(true);
                var placement = InPlayAdRegistry.GetPlacement(this);
                if (placement != null) _MoveAdToPlacement(_currentAd, placement);
                _tryIndex++;
                _tryIndex %= _priorityList.Count;
            }
        }
#endif

        public void OnPotentialProviderChanged(InPlayAdProvider provider)
        {
            if (_currentAd == null) return;
            if (provider == InPlayAdProvider.None) return;
            var removedText = provider == InPlayAdProvider.Adverty ? "Gadsme" : "Adverty";
            if (!_currentAd.ClassName.Contains(removedText)) return;
            this.LogInfo(SharedLogTag.InPlayAds, nameof(provider), provider, "f", nameof(OnPotentialProviderChanged), "remove", $"_currentAd.ClassName.Contains({removedText})");
            _RemoveCurrentAdIfExisted();
            var removedKeys = (from e in _adDict where e.Key.Contains(removedText) select e.Key).ToList();
            foreach (var key in removedKeys)
            {
                var ad = _adDict[key];
                _adDict.Remove(key);
                Destroy(ad.gameObject);
            }
        }
    }
}