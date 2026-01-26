// using Newtonsoft.Json;
// using Shared.Tracking.Models.Templates;
// using Shared.Tracking.Templates;
// using UnityEngine;
//
// namespace Shared.Tracking.Internal.Handler
// {
//     public class WebFirebaseTrackingHandler : ITrackingHandler
//     {
//         public void Handle(ITrackingEvent e)
//         {
// #if FIREBASE
//             if (e is IFirebaseCustomEvent ee)
//             {
//                 Firebase.Analytics.FirebaseAnalytics.LogEvent(name: e.EventName, parameters: ee.ToFirebaseParams());
//             }
// #endif
// #if FIREBASE_WEBGL
//             if (e is IFirebaseCustomEvent eee)
//             {
//                 var p = JsonConvert.SerializeObject(eee.ToFirebaseWebParams());
//                 Application.ExternalCall("logEvents", e.EventName, p);
//             }
// #endif
//         }
//     }
// }