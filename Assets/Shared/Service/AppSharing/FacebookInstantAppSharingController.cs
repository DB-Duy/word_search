#if FACEBOOK_INSTANT && APP_SHARE
using System.Runtime.InteropServices;
using Shared.Utils;
using UnityEngine;

namespace Shared.AppSharing
{
    public class FacebookInstantAppSharingController: IAppSharingController
    {
        private const string Tag = "FacebookInstantAppSharingController";
        
        [DllImport("__Internal")]
        private static extern void InvokeFacebookInstantShareDialog();

        public void Share()
        {
            SharedLogger.Log($"{Tag}->Share");
            if(Application.isEditor) return;
            InvokeFacebookInstantShareDialog();
        }
    }
}
#endif