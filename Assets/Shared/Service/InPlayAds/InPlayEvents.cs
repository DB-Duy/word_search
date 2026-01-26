using System;
using Shared.View.InPlayAds;

namespace Shared.Service.InPlayAds
{
    public static class InplayEvents
    {
        public static Action<IInPlayAd> InPlayAdClicked = delegate { };
    }
}