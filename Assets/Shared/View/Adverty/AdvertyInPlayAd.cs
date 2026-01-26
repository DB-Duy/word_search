#if ADVERTY_4
using Adverty;
using UnityEngine;
#endif

using Shared.Utils;
using Shared.View.InPlayAds;

namespace Shared.View.Adverty
{
    public abstract class AdvertyInPlayAd : AbstractInPlayAd, ISharedUtility
    {
        public override bool IsReady { get; protected set; }

#if ADVERTY_4
        protected override void Awake()
        {
            base.Awake();
            AdvertyEvents.AdDelivered += _OnAdDelivered;
        }

        private void _OnAdDelivered(BaseUnit unit)
        {
            this.LogInfo(SharedLogTag.InPlayAdsNAdverty, nameof(name), name, nameof(unit.name), unit.name);
            if (gameObject != unit.gameObject) return;
            IsReady = true;
            var u = GetComponent<InPlayUnit>();
            u.GetComponent<SpriteRenderer>();
        }
#endif
    }
}