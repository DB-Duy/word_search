// using System.Collections.Generic;
// using Newtonsoft.Json;
// using Shared.Service.S2S.Request;
// using Shared.Utils;
//
// namespace Shared.IAP.RemoteVerification.Request
// {
//     /// <summary>
//     /// https://developer.apple.com/documentation/appstorereceipts/validating_receipts_on_the_device
//     /// </summary>
//     public class AppStoreRequest : IRequest
//     {
//         private const string K_TRANSACTION_ID = "transactionId";
//         private const string K_RECEIPT = "receipt";
//         private const string K_USER_ID = "userId";
//         private const string K_ADID = "adid";
//         private const string K_FIREBASE_APP_INSTANCE_ID = "firebaseAppInstanceId";
//         private const string K_APP_METRICA_PROFILE_ID = "appMetricaProfileId";
//         private const string K_IDFA = "idfa";
//         private const string K_IDFV = "idfv";
//         
//         private readonly Dictionary<string, object> p = new();
//         private readonly Dictionary<string, object> userDataDict = new();
//
//         public string BuildJsonString()
//         {
//             p.Upsert("userData", userDataDict);
//             return JsonConvert.SerializeObject(p);
//         }
//
//         public AppStoreRequest(string transactionId, string receipt)
//         {
//             p.Upsert(K_TRANSACTION_ID, transactionId);
//             p.Upsert(K_RECEIPT, receipt);
//         }
//         
//         public AppStoreRequest WithUserId(string userId)
//         {
//             userDataDict.Upsert(K_USER_ID, userId);
//             return this;
//         }
//         
//         public AppStoreRequest WithAdId(string adid)
//         {
//             userDataDict.Upsert(K_ADID, adid);
//             return this;
//         }
//         
//         public AppStoreRequest WithIdfa(string idfa)
//         {
//             userDataDict.Upsert(K_IDFA, idfa);
//             return this;
//         }
//         
//         public AppStoreRequest WithIdfv(string idfv)
//         {
//             userDataDict.Upsert(K_IDFV, idfv);
//             return this;
//         }
//         
//         public AppStoreRequest WithFirebaseAppInstanceId(string firebaseAppInstanceId)
//         {
//             userDataDict.Upsert(K_FIREBASE_APP_INSTANCE_ID, firebaseAppInstanceId);
//             return this;
//         }
//         
//         public AppStoreRequest WithAppMetricaProfileId(string appMetricaProfileId)
//         {
//             userDataDict.Upsert(K_APP_METRICA_PROFILE_ID, appMetricaProfileId);
//             return this;
//         }
//     }
// }