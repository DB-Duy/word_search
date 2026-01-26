#if UNITY_LOCALIZATION && REWARDED_AD_MESSAGE_LOCALIZATION_RESOLVER
using System.Collections.Generic;
using Shared.Core.IoC;
using Shared.Utils;
using UnityEngine.Localization.Settings;

namespace Shared.Service.Ads.Common
{
    [Component]
    public class RewardedAdLocalizationFailMessageResolver : IRewardedAdFailMessageResolver
    {
        private const string Tag = "RewardedAdLocalizationFailMessageResolver";
        private const string LocalizationTable = "DefaultStringTableCollection";
        private const string LocalizationInternet = "rv.no_internet_connection";
        private const string LocalizationAds = "rv.not_available";

        private readonly string _localizationTable;
        private readonly Dictionary<RewardedAdShowFailReason, string> _keyMap;
        
        public RewardedAdLocalizationFailMessageResolver()
        {
            _localizationTable = LocalizationTable;
            _keyMap = new Dictionary<RewardedAdShowFailReason, string>
            {
                { RewardedAdShowFailReason.NoInternetConnection, LocalizationInternet },
                { RewardedAdShowFailReason.NotAvailable, LocalizationAds },
            };
        }

        public RewardedAdLocalizationFailMessageResolver(string localizationTable, Dictionary<RewardedAdShowFailReason, string> keyMap)
        {
            _localizationTable = localizationTable;
            _keyMap = keyMap;
        }

        public string Resolve(RewardedAdShowFailReason reason, string defaultValue = null)
        {
            if (!_keyMap.ContainsKey(reason)) return string.Empty;
            var key = _keyMap[reason];
            var response = LocalizationSettings.Instance.GetStringDatabase().GetLocalizedString(_localizationTable, key);
            SharedLogger.Log($"{Tag}->Resolve: {reason.ToString()}={reason}");
            return response;
        }
    }
}
#endif