// using System;
// using Newtonsoft.Json;
// using Shared.Common;
// using UnityEngine;
//
// namespace Shared.IAP.HuaweiIap
// {
//     public class HuaweiRestorePurchaseAsyncOperation : IHuaweiRestorePurchaseAsyncOperation
//     {
//         public Action OnCompleteEvent { get; set; }
//         [JsonProperty("isComplete")] public bool IsComplete => (_isRestorePurchaseRecordsComplete && _isRestoreOwnedPurchasesComplete) || IsTimeout;
//         [JsonProperty("isSuccess")] public bool IsSuccess => IsComplete;  
//         [JsonProperty("failReason")] public string FailReason { get; private set; }
//
//         [JsonProperty("restorePurchaseRecordsCompleteCount")] private int _restorePurchaseRecordsCompleteCount = 0;
//         [JsonProperty("restoreOwnedPurchasesCompleteCount")] private int _restoreOwnedPurchasesCompleteCount = 0;
//
//         [JsonProperty("isRestorePurchaseRecordsComplete")] private bool _isRestorePurchaseRecordsComplete = false;
//         [JsonProperty("isRestoreOwnedPurchasesComplete")] private bool _isRestoreOwnedPurchasesComplete = false;
//         
//         [JsonProperty("timeOutInSeconds")] private float _timeOutInSeconds;
//
//         [JsonProperty("startTime")] private float _startTime = Time.realtimeSinceStartup;
//         [JsonProperty("isTimeout")]  private bool IsTimeout => (Time.realtimeSinceStartup - _startTime) >= _timeOutInSeconds;
//         
//         public HuaweiRestorePurchaseAsyncOperation(float timeOutInSeconds = 3f)
//         {
//             _timeOutInSeconds = timeOutInSeconds;
//         }
//
//         public IAsyncOperation Success()
//         {
//             throw new System.NotImplementedException();
//         }
//
//         public IAsyncOperation Fail(string reason)
//         {
//             throw new System.NotImplementedException();
//         }
//
//         public void RestorePurchaseRecordsComplete()
//         {
//             _restorePurchaseRecordsCompleteCount++;
//             _isRestorePurchaseRecordsComplete = _restorePurchaseRecordsCompleteCount >= 3;
//         }
//
//         public void RestorePurchaseRecordsFailure()
//         {
//             RestorePurchaseRecordsComplete();
//         }
//
//         public void RestoreOwnedPurchasesComplete()
//         {
//             _restoreOwnedPurchasesCompleteCount++;
//             _isRestoreOwnedPurchasesComplete = _restoreOwnedPurchasesCompleteCount >= 3;
//         }
//
//         public void RestoreOwnedPurchasesFailure()
//         {
//             RestoreOwnedPurchasesComplete();
//         }
//
//         public override string ToString() => JsonConvert.SerializeObject(this);
//     }
// }