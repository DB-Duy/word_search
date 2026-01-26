#if GOOGLE_PLAY
using Shared.Core.Handler;
using Shared.Core.IoC;
using Shared.Referrer;
using Shared.Tracking.Templates;

namespace Shared.Service.Tracking.EventModify
{
    [Component]
    public class TrackingEventGooglePlayReferrerModifier : IHandler<ITrackingEvent>
    {
        private const string KUtmSource = "utm_source";
        private const string KUtmMedium = "utm_medium";
        private const string KUtmTerm = "utm_term";
        private const string KUtmContent = "utm_content";
        private const string KUtmCampaign = "utm_campaign";
        
        private readonly IInstallReferrerController _installReferrerController;

        // public TrackingEventGooglePlayReferrerModifier(IInstallReferrerController installReferrerController)
        // {
        //     _installReferrerController = installReferrerController;
        // }

        public void Handle(ITrackingEvent e)
        {
            if (e is not IExParamsEvent ee) return;
            InstallReferrer referrer = null;
            if (_installReferrerController is GooglePlayInstallReferrerController { InstallReferrer: not null } googleReferrer) referrer = googleReferrer.InstallReferrer;
            ee.AddParams(KUtmSource, referrer == null ? string.Empty : referrer.UtmSource);
            ee.AddParams(KUtmMedium, referrer == null ? string.Empty : referrer.UtmMedium);
            ee.AddParams(KUtmTerm, referrer == null ? string.Empty : referrer.UtmTerm);
            ee.AddParams(KUtmContent, referrer == null ? string.Empty : referrer.UtmContent);
            ee.AddParams(KUtmCampaign, referrer == null ? string.Empty : referrer.UtmCampaign);
        }
    }
}
#endif