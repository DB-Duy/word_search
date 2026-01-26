using Shared.Service.Ads.Common;
using UnityEngine;

namespace Shared.Service.Ads
{
    public interface IMrecAd
    {
        void Show(IAdPlacement placement);
        void Hide(IAdPlacement placement);
        void Register(IAdPlacement placement, Vector2 position);
    }
}