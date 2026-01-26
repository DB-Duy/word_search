#if UNITY_ANDROID && FYBER && !UNITY_EDITOR
using Shared.Core.Handler;
using Shared.Core.IoC;
using Shared.Entity.Ump;
using Shared.Service.Fyber;
using Shared.Utils;

namespace Shared.Service.Ump.Handlers.Android
{
    [Component]
    public class FyberUmpValueSyncHandler : IHandler<UmpEntity>, ISharedUtility, IUmpValueSyncHandler
    {
        private readonly ISharedFyber _sharedFyber = new AndroidFyber();

        public void Handle(UmpEntity e)
        {
            if (!e.GdprApply)
            {
                this.LogInfo(SharedLogTag.UmpNFyber, nameof(e.GdprApply), e.GdprApply);
                return;
            }
            this.LogInfo(nameof(e), e);

            _sharedFyber.SetGdprConsent(e.GdprApply);
            _sharedFyber.SetGdprConsentString(e.TcString);
            _sharedFyber.SetUsPrivacyString(e.UsPrivacyString);
        }
    }
}
#endif