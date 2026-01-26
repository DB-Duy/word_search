#if INMOBI && UNITY_ANDROID

using System.Collections.Generic;
using Shared.Core.Handler;
using Shared.Core.IoC;
using Shared.Entity.Ump;
using Shared.Service.InMobi;
using Shared.Utils;
using UnityEngine;

namespace Shared.Service.Ump.Handlers
{
    [Component]
    public class InMobiUmpValueSyncHandler : IHandler<UmpEntity>, ISharedUtility, IUmpValueSyncHandler
    {
        private static readonly ISharedInMobi _sharedInMobi = new AndroidInMobi();

        public void Handle(UmpEntity e)
        {
            var gdprConsentObject = new Dictionary<string, object>
            {
                { InMobiConstants.IM_GDPR_CONSENT_AVAILABLE, true },
                { InMobiConstants.IM_GDPR_CONSENT_GDPR_APPLIES, e.GdprApplies },
                { InMobiConstants.IM_GDPR_CONSENT_IAB, e.TcString }
            };

            _sharedInMobi.UpdateGdprConsent(gdprConsentObject);
            _sharedInMobi.SetUSPrivacyString(e.UsPrivacyString);

            this.LogInfo(SharedLogTag.UmpNInMobi, nameof(gdprConsentObject), gdprConsentObject);
        }
    }
}
#endif