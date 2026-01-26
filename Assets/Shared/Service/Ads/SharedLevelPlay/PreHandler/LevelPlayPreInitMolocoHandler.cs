// #if LEVEL_PLAY && LEVEL_PLAY_MOLOCO
// using System.Collections;
// using MolocoSdk;
// using Shared.Core.Handler.Corou;
// using Shared.Core.IoC;
// using Shared.Utils;
//
// namespace Shared.Service.Ads.LevelPlay.PreHandler
// {
//     [Component]
//     public class LevelPlayPreInitMolocoHandler : ICoroutineHandler, ISharedUtility, ILevelPlayPreInitHandler
//     {
//         public IEnumerator Handle()
//         {
//             var privacy = new PrivacySettings
//             {
//                 IsUserConsent = true,
//                 IsAgeRestrictedUser = false,
//                 IsDoNotSell = false
//             };
//             MolocoSDK.SetPrivacy(privacy);
//             this.LogInfo(SharedLogTag.AdNLevelPlayNMoloco, nameof(privacy), privacy);
//             yield break;
//         }
//     }
// }
// #endif