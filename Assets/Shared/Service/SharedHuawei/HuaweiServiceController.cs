#if HUAWEI
using HmsPlugin;
using HuaweiMobileServices.Crash;
using Shared.Common;
using UnityEngine;

namespace Shared.SharedHuawei
{
    public class HuaweiServiceController : IHuaweiServiceController
    {
        public bool IsInitialized { get; private set; }
        private IAsyncOperation _initOperation;
        
        public IAsyncOperation Initialize()
        {
            if (_initOperation != null) return _initOperation;
            if (!Application.isEditor)
            {
                HMSAPMManager.Instance.EnableCollection(true);
                HMSAPMManager.Instance.EnableAnrMonitor(true);
                AGConnectCrash.GetInstance().EnableCrashCollection(true);
                HMSGameServiceManager.Instance.Init();
            }
            IsInitialized = true;
            _initOperation = new SharedAsyncOperation().Success();
            return _initOperation;
        }
        
        
//         [Conditional("LOG_INFO")]
//         private void _PrintAAID()
//         {
//             var inst = HmsInstanceId.GetInstance();
//             var idResult = inst.AAID;
//             idResult.AddOnSuccessListener((result) =>
//             {
// #if LOG_INFO
//                 Debug.LogFormat("{0} -> GetAAID: result.Id:{1}", TAG, result.Id);
// #endif
//                 //AAIDResult AAIDResult = result;
//                 //Debug.Log("AppMessaging: " + result.Id);
//                 //AAIDResultAction?.Invoke(result);
//             }).AddOnFailureListener((exception) =>
//             {
//
//             });
//         }
    }
}
#endif