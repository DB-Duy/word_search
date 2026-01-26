// #if FIREBASE
// using Shared.Core.Handler;
// using Shared.Core.IoC;
// using Shared.Entity.Session;
// using Shared.Service.FirebaseRemoteConfig;
// using Shared.Utils;
//
// namespace Shared.Service.Session.Handler
// {
//     public class FirebaseRemoteConfigNewSessionHandler : IHandler<SessionEntity>, IIoC
//     {
//         private const string Tag = "FirebaseRemoteConfigNewSessionHandler";
//         
//         public void Handle(SessionEntity sessionEntity)
//         {
//             SharedLogger.LogJson(SharedLogTag.SessionNRemoteConfig, $"{Tag}->Handle", nameof(sessionEntity), sessionEntity);
//             this.Resolve<MobileFirebaseRemoteConfigService>().Fetch();
//         }
//     }
// }
// #endif