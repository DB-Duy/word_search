#if FACEBOOK_INSTANT
using System.Runtime.InteropServices;
using Shared.Utils;
using UnityEngine;

namespace Shared.SharedShortcut
{
    public class SharedFacebookInstantShortcutController : ISharedShortcutController
    {
        private const string TAG = "SharedFacebookInstantShortcutController";
        
        [DllImport("__Internal")] private static extern void CreateShortcutFacebookInstant();
        
        public void CreateShortcut()
        {
            SharedLogger.Log($"{TAG}->CreateShortcut");
            if(Application.isEditor) return;
            CreateShortcutFacebookInstant();
        }
    }
}
#endif