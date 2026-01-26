using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using Shared.Core.Repository.RemoteConfig;
using Shared.Repository.RemoteConfig;
using Shared.SharedReport.Version;
using Shared.SharedReport.Version.Android;

#if LEVEL_PLAY
using Shared.SharedReport.Version.UnitySdk;
#endif
using Shared.Utils;
using UnityEngine;
using UnityEngine.Networking;

namespace Shared.SharedReport
{
    public class SharedVersionReport : ISharedVersionReport
    {
        private const string TAG = "SharedVersionReport";
        private MonoBehaviour CoroutineBehaviour { get; }
        private readonly List<IVersionGetter> _getters = new();
        private readonly IRemoteConfigRepository<VersionReportConfig> _configRepository;

        private SharedVersionReport(MonoBehaviour coroutineBehaviour, IRemoteConfigRepository<VersionReportConfig> configRepository)
        {
            CoroutineBehaviour = coroutineBehaviour;
            _configRepository = configRepository;
        }

        public void Add(IVersionGetter getter)
        {
            _getters.Add(getter);
        }

        public void Report()
        {
            CoroutineBehaviour.StartCoroutine(_Report());
        }

        private IEnumerator _Report()
        {
            Dictionary<string, Dictionary<string, string>> result = new();
            foreach (var getter in _getters)
            {
                var operation = getter.Get();
                while (!operation.IsComplete)
                {
                    yield return null;    
                }

                var versionEntity = operation.Result;
                if (result.ContainsKey(versionEntity.Name))
                {
                    result[versionEntity.Name].AddRange(versionEntity.Versions);
                }
                else
                {
                    result.Add(versionEntity.Name, versionEntity.Versions);    
                }
            }
            SharedLogger.Log($"{TAG}->_Report: {JsonConvert.SerializeObject(result)}");
            if (_configRepository != null)
            {
                var config = _configRepository.Get();
                var request = VersionReportUtils.NewInstance(result);
                var jsonData = JsonConvert.SerializeObject(request);
                SharedLogger.Log($"{TAG}->_Report: {config.Endpoint} {jsonData}", SharedLogTag.Version);
                using var www = UnityWebRequest.Post(config.Endpoint, jsonData, "application/json");
                www.SetRequestHeader("x-api-key", config.ApiKey);
                yield return www.SendWebRequest();

                if (www.result != UnityWebRequest.Result.Success)
                {
                    SharedLogger.LogError($"{TAG}->_Report: www.result={www.result} {www.error}", SharedLogTag.Version);
                }
                else
                {
                    var responseString = www.downloadHandler.text;
                    SharedLogger.Log($"{TAG}->_Report: {responseString}", SharedLogTag.Version);
                }
            }
        }

        public static SharedVersionReport NewDefaultInstance(MonoBehaviour coroutineBehaviour, IRemoteConfigRepository<VersionReportConfig> configRepository)
        {
            var report = new SharedVersionReport(coroutineBehaviour, configRepository);
            
            var versionDefine = VersionDefine.NewInstance();
            foreach (var kv in versionDefine.Versions)
            {
                var getter = new AndroidSdkVersionGetter(coroutineBehaviour, kv.Key);
                if (!string.IsNullOrEmpty(kv.Value.AndroidNativeAccessPoint))
                {
                    getter.AddConfig("android_native", kv.Value.AndroidNativeAccessPoint);
                }
                
                if (!string.IsNullOrEmpty(kv.Value.AndroidAdapterAccessPoint))
                {
                    getter.AddConfig("android_adapter", kv.Value.AndroidAdapterAccessPoint);
                }
                
                report.Add(getter);    
            }
#if LEVEL_PLAY
            report.Add(new LevelPlayVersionGetter(coroutineBehaviour));
#endif
#if APS
            report.Add(new ApsVersionGetter(coroutineBehaviour));
#endif
            return report;
        }
    }
}