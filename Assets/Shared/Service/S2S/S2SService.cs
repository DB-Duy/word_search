#if S2S
using System;
using System.Collections;
using Newtonsoft.Json;
using Shared.Core.Async;
using Shared.Core.IoC;
#if ADJUST
using Shared.Repository.Adjust;
#endif
using Shared.Repository.AppMetrica;
using Shared.Repository.Firebase;
using Shared.Repository.S2S;
using Shared.Service.S2S.Request;
using Shared.Service.S2S.Response;
using Shared.Service.SharedCoroutine;
using Shared.Utils;
using UnityEngine;
using UnityEngine.Networking;
using Zenject;

namespace Shared.Service.S2S
{
    [Service]
    public class S2SService : IS2SService, ISharedUtility
    {
        private const string Tag = "S2SService";
        
        [Inject] private S2SConfigRepository _configRepository;
        [Inject] private FirebaseRepository _firebaseRepository;
        [Inject] private AppMetricaRepository _appMetricaRepository;
#if ADJUST
        [Inject] private AdjustRepository _adjustRepository;
#endif

        public UserDataRequest GetUserData()
        {
#if ADJUST
            var firebaseEntity = _firebaseRepository.Get();
            var appMetricaEntity = _appMetricaRepository.Get();
            var adjustEntity = _adjustRepository.Get();
            this.LogInfo(SharedLogTag.S2SNIap_, nameof(firebaseEntity), firebaseEntity, nameof(appMetricaEntity), appMetricaEntity, nameof(adjustEntity), adjustEntity);
            
            var r = new UserDataRequest
            {
                UserId = SystemInfo.deviceUniqueIdentifier,
                AdjustId = adjustEntity.AdjustId,
                GpsAdId = adjustEntity.GoogleAdId,
                AndroidId = this.GetAndroidId(),
                FirebaseAppInstanceId = firebaseEntity.AnalyticsInstanceId,
                AppMetricaProfileId = appMetricaEntity.UserProfileID
            };
            return r;
#else
            var firebaseEntity = _firebaseRepository.Get();
            var appMetricaEntity = _appMetricaRepository.Get();
            this.LogInfo(SharedLogTag.S2SNIap_, nameof(firebaseEntity), firebaseEntity, nameof(appMetricaEntity), appMetricaEntity);
            
            var r = new UserDataRequest
            {
                UserId = SystemInfo.deviceUniqueIdentifier,
                AdjustId = null,
                GpsAdId = null,
                FirebaseAppInstanceId = firebaseEntity != null ? firebaseEntity.AnalyticsInstanceId : null,
                AppMetricaProfileId = appMetricaEntity != null ? appMetricaEntity.UserProfileID : null
            };
#if UNITY_ANDROID
            r.AndroidId = this.GetAndroidId();
#elif UNITY_IOS
            r.AndroidId = null;
#endif
            return r;
#endif
        }

        public IResultAsyncOperation<T> ExecutePost<T>(object o)
        {
            var asyncOperation = new ResultAsyncOperationImpl<T>();
            this.StartSharedCoroutine(_ExecutePost<T>(o, (data, errorMessage) =>
            {
                if (!string.IsNullOrEmpty(errorMessage))
                {
                    asyncOperation.Fail(errorMessage);
                    return;
                }
                asyncOperation.SuccessWithResult(data);
            }));
            return asyncOperation;
        }

        private IEnumerator _ExecutePost<T>(object o, Action<T, string> onComplete)
        {
            var config = _configRepository.Get();
            if (!config.Unlocked)
            {
                SharedLogger.LogJson(SharedLogTag.S2SNIap, $"{Tag}->_ExecutePost", nameof(config.Unlocked), config.Unlocked);
                onComplete.Invoke(default, "!config.Unlocked");
                yield break;
            }

            var jsonData = JsonConvert.SerializeObject(o);
            SharedLogger.LogJson(SharedLogTag.S2SNIap, $"{Tag}->_ExecutePost", nameof(config.RemoteUrl), config.RemoteUrl, nameof(jsonData), jsonData);
            
            using var www = UnityWebRequest.Post(config.RemoteUrl, jsonData, "application/json");
            www.SetRequestHeader("x-api-key", config.ApiKey);
            yield return www.SendWebRequest();

            SharedLogger.LogJson(SharedLogTag.S2SNIap, $"{Tag}->_ExecutePost", nameof(config.RemoteUrl), config.RemoteUrl, nameof(jsonData), jsonData, nameof(www.result), www.result);
            if (www.result != UnityWebRequest.Result.Success)
            {
                SharedLogger.LogError($"{Tag}->_Validate: www.result={www.result}", SharedLogTag.Iap);
                onComplete.Invoke(default, $"www.result={www.result}");
                yield break;
            }
            // Form upload complete! {"status":{"message":"Success","code":200,"success":true},"entity":{"isProduction":false,"errorMessage":null}}
            var responseString = www.downloadHandler.text;
            SharedLogger.LogJson(SharedLogTag.S2SNIap, $"{Tag}->_ExecutePost", nameof(config.RemoteUrl), config.RemoteUrl, nameof(jsonData), jsonData, nameof(www.result), www.result, nameof(responseString), responseString);
            var response = S2SResponse<T>.NewInstance<T>(responseString);
            onComplete.Invoke(response.Data, null);
        }
    }
}
#endif