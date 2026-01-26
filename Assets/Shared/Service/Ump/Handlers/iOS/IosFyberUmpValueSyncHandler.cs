#if UNITY_IOS  && FYBER
using Shared.Core.IoC;
using Shared.Entity.Ump;
using Shared.Service.Fyber;
using Shared.Utils;
using Zenject;

namespace Shared.Service.Ump.Handlers.iOS
{
    [Component]
    public class IosFyberUmpValueSyncHandler : IUmpValueSyncHandler
    {
        [Inject(Optional = true)] private ISharedFyber _fyber;
     
        public void Handle(UmpEntity e)
        {
            if (!e.GdprApply)
            {
                this.LogInfo(SharedLogTag.UmpNFyber, nameof(e.GdprApply), e.GdprApply, "status", "IGNORED");
                return;
            }

            if (_fyber == null)
            {
                this.LogInfo(SharedLogTag.UmpNFyber, nameof(_fyber), "null", "status", "IGNORED");
                return;
            }

            this.LogInfo(nameof(e), e);
            _fyber.SetGdprConsent(e.GdprApply);
            _fyber.SetGdprConsentString(e.TcString);
            _fyber.SetUsPrivacyString(e.UsPrivacyString);
        }
    }
}
#endif