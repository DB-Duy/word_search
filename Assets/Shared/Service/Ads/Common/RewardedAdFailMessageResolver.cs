using System.Collections.Generic;
using Shared.Utils;

namespace Shared.Service.Ads.Common
{
    public class RewardedAdFailMessageResolver : IRewardedAdFailMessageResolver
    {
        private readonly string _other;
        private readonly Dictionary<RewardedAdShowFailReason, string> _messageDict;

        public RewardedAdFailMessageResolver(string other = "No Video Ads!", string notAvailable = "No Video Ads!", string noInternet = "No Internet Connection!")
        {
            _other = other;
            _messageDict = new Dictionary<RewardedAdShowFailReason, string>
            {
                { RewardedAdShowFailReason.NoInternetConnection, noInternet },
                { RewardedAdShowFailReason.NotAvailable, notAvailable }
            };
        }
 
        public string Resolve(RewardedAdShowFailReason reason, string defaultValue = null)
        {
            return _messageDict.Get(reason, _other);
        }
    }
}