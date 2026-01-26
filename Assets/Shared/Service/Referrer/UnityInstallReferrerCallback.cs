#if GOOGLE_PLAY
using System;
using UnityEngine;

namespace Shared.Referrer
{
    public class UnityInstallReferrerCallback : AndroidJavaProxy, IUnityInstallReferrerCallback
    {
        public UnityInstallReferrerCallback() : base("com.unity3d.player.IUnityInstallReferrerCallback")
        {
        }
        
        public event Action<int> OnInstallReferrerSetupFinishedEvent = delegate { };
        public event Action OnInstallReferrerServiceDisconnectedEvent = delegate { };

        void onInstallReferrerSetupFinished(int responseCode)
        {
            OnInstallReferrerSetupFinishedEvent.Invoke(responseCode);
        }

        void onInstallReferrerServiceDisconnected()
        {
            OnInstallReferrerServiceDisconnectedEvent.Invoke();
        }
    }
}
#endif