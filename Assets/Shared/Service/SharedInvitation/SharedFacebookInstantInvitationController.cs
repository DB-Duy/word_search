#if FACEBOOK_INSTANT
using System.Runtime.InteropServices;
using Shared.Utils;
using UnityEngine;

namespace Shared.SharedInvitation
{
    public class SharedFacebookInstantInvitationController : ISharedInvitationController
    {
        private const string TAG = "SharedFacebookInstantInvitationController";
        
        [DllImport("__Internal")] private static extern void InvokeInviteDialog();
        
        public void ShowPlatformInviteDialog()
        {
            SharedLogger.Log($"{TAG}->ShowPlatformInviteDialog");
            if(Application.isEditor) return;
            InvokeInviteDialog();
        }
    }
}
#endif