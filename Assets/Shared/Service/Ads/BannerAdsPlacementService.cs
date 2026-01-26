using System.Collections.Generic;
using Shared.Core.IoC;
using Shared.View.Ad;
using Zenject;

namespace Shared.Service.Ads
{
    [Service]
    public class BannerAdsPlacementService : IBannerAdsPlacementService
    {
        [Inject] private IAdService _adService;
        
        private readonly HashSet<BannerAdsPlacement> _adsPlacementList = new();
        
        public void Register(BannerAdsPlacement p)
        {
            _adsPlacementList.Add(p);
        }

        public void Remove(BannerAdsPlacement p)
        {
            _adsPlacementList.Remove(p);
        }

        public bool IsVisible()
        {
            return _adService.BannerAd.Validate();
        }

        public void ValidateAndNotifyChanges()
        {
            var e = _adService.BannerAd.Validate();
            foreach (var p in _adsPlacementList) p.UpdateView(e);
        }
    }
}