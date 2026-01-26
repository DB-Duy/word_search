#if GOOGLE_PLAY
using System;

namespace Shared.Referrer
{
    public interface IUnityInstallReferrerCallback
    {
        event Action<int> OnInstallReferrerSetupFinishedEvent;
        event Action OnInstallReferrerServiceDisconnectedEvent;
    }
}
#endif