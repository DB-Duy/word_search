#if ODEEO_AUDIO
using Shared.Core.Handler;
using Shared.Core.IoC;
using Shared.Entity.Ump;
using Shared.Service.Odeeo;

namespace Shared.Service.Ump.Handlers
{
    [Component]
    public class OdeeoUmpValueSyncHandler : IHandler<UmpEntity>, ISharedUtility, IUmpValueSyncHandler
    {
        public void Handle(UmpEntity e)
        {
            // if (e.GdprApply) 
            //     OdeeoUtilities.SetGdprConsent(true, e.TcString); 
            // else 
            //     OdeeoUtilities.SetGdprConsent(true);
            OdeeoUtilities.SetGdprConsent(true);
            OdeeoUtilities.SetDoNotSell(e.UsPrivacyString == UsPrivacyValue.Const1YynOn, e.UsPrivacyString);
        }
    }
}
#endif