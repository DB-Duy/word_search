// #if LEVEL_PLAY && BIGO && UNITY_ANDROID
// using System.Collections;
// using Shared.Core.Handler.Corou;
// using Shared.Core.IoC;
// using Shared.Entity.Ump;
// using Shared.Repository.Ump;
// using Shared.Utils;
// using UnityEngine;
//
// namespace Shared.Service.Ads.LevelPlay.PreHandler
// {
//     [Component]
//     public class LevelPlayPreInitBigoHandler : ICoroutineHandler, ISharedUtility, IIoC, ILevelPlayPreInitHandler
//     {
//         private readonly AndroidJavaClass _javaClass = new("sg.bigo.ads.BigoAdSdk");
//         
//         //Fixed Android memory leak (global reference table overflow (max=51200)global reference table dump)
//         private bool _IsInitSuccess() => _javaClass.CallStatic<bool>("isInitialized");
//         
//         public IEnumerator Handle()
//         {
//             while (!_IsInitSuccess()) 
//                 yield return null;
//             
//             this.LogInfo(SharedLogTag.AdNLevelPlayNBigo);
//             
//             var usPrivacyStringRepository = this.Resolve<UsPrivacyStringRepository>();
//             var usPrivacy = usPrivacyStringRepository.Get();
//             var userConsent = string.IsNullOrEmpty(usPrivacy) || usPrivacy.Equals(UsPrivacyValue.Const1YnnOff);
//             
//             var currentActivity = this.GetCurrentActivity();
//             var consentOptionsClass = new AndroidJavaClass("sg.bigo.ads.ConsentOptions"); // Replace with the actual package name of the enum
//             var gdpr = consentOptionsClass.GetStatic<AndroidJavaObject>("GDPR");
//             var ccpa = consentOptionsClass.GetStatic<AndroidJavaObject>("CCPA");
//             
//             _javaClass.CallStatic("setUserConsent", currentActivity, gdpr, true);
//             _javaClass.CallStatic("setUserConsent", currentActivity, ccpa, userConsent);
//         }
//     }
// }
// #endif