using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Newtonsoft.Json;
using Shared.Utilities;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace Shared.Utils
{
    public static class SharedLogger
    {
        private static int _logCount = 0;
        
        [System.Serializable]
        public class LogArray<T>
        {
            [JsonProperty("arr")] private List<T> _array;

            public LogArray(List<T> array)
            {
                _array = array;
            }
        }

        // [Conditional("LOG_INFO")]
        // private static void IncreaseLogCount()
        // {
        //     _logCount++;
        //     if (_logCount % 30 == 0) LogVersion(); 
        // }

        // [Conditional("LOG_INFO")]
        // private static void LogVersion()
        // {
        //     var versionString = $"{Application.productName} Version {SharedApplication.VersionName}({SharedApplication.VersionCode})";
        //     Log(versionString);
        // }
        
        private static string _ConvertToJsonString(params object[] jsonParams)
        {
            if (jsonParams is { Length: 0 }) return string.Empty;
            Dictionary<string, object> p = new();
            var maxLength = jsonParams.Length % 2 == 0 ? jsonParams.Length : jsonParams.Length - 1;
            maxLength--;
            for (var i = 0; i < maxLength; i += 2)
            {
                var key = jsonParams[i].ToString();
                if (p.ContainsKey(key)) key += i.ToString();
                p.Add(key, jsonParams[i + 1]);
            }

            return JsonConvert.SerializeObject(p);
        }

        [Conditional("LOG_INFO")]
        public static void LogJson(string logMsg, params object[] jsonParams)
        {
            var jsonString = _ConvertToJsonString(jsonParams);
            Log($"{logMsg}: {jsonString}", null);
        }
        
        [Conditional("LOG_INFO")]
        public static void LogJson(List<string> tags, string logMsg, params object[] jsonParams)
        {
            var jsonString = _ConvertToJsonString(jsonParams);
            Log($"{logMsg}: {jsonString}", tags.ToArray());
        }
        
        [Conditional("LOG_INFO")]
        public static void LogJson(string tag, string logMsg, params object[] jsonParams)
        {
            var jsonString = _ConvertToJsonString(jsonParams);
            Log($"{logMsg}: {jsonString}", tag);
        }

        [Conditional("LOG_INFO")]
        public static void Log(string logMsg)
        {
            LogProduction(logMsg);
        }

        private static string _Convert(params string[] logFilters) => logFilters is { Length: > 0 } ? $"[{string.Join(",", logFilters)}] " : string.Empty;

        public static void LogProduction(string logMsg, params string[] logFilters)
        {
            var tags = _Convert(logFilters);
#if UNITY_ANDROID
            var arr = logMsg.SplitIntoChunks(800);
            foreach (var str in arr) UnityEngine.Debug.Log($"{tags}{str}");
#else
            UnityEngine.Debug.Log($"{tags}{logMsg}");
#endif
        }
        
        [Conditional("LOG_INFO")]
        public static void Log(string logMsg, params string[] logFilters)
        {
            var tags = _Convert(logFilters);
#if UNITY_ANDROID
            var arr = logMsg.SplitIntoChunks(800);
            foreach (var str in arr) UnityEngine.Debug.Log($"{tags}{str}");
#else
            UnityEngine.Debug.Log($"{tags}{logMsg}");
#endif
        }

        [Conditional("LOG_INFO")]
        public static void LogError(string logMsg, params string[] logFilters)
        {
            var tags = _Convert(logFilters);
#if UNITY_ANDROID
            var arr = logMsg.SplitIntoChunks(800);
            foreach (var str in arr) UnityEngine.Debug.LogError($"{tags}{str}");
#else
            UnityEngine.Debug.LogError($"{tags}{logMsg}");
#endif
        }
        
        [Conditional("LOG_INFO")]
        public static void LogJsonError(string tag, string logMsg, params object[] jsonParams)
        {
            var jsonString = _ConvertToJsonString(jsonParams);
            LogError($"{logMsg} {jsonString}", tag);
        }
        
        [Conditional("LOG_INFO")]
        public static void LogJsonError(List<string> tags, string logMsg, params object[] jsonParams)
        {
            var jsonString = _ConvertToJsonString(jsonParams);
            LogError($"{logMsg} {jsonString}", tags.ToArray());
        }
        
        [Conditional("LOG_INFO")]
        public static void LogError(this object o, params object[] jsonParams)
        {
            var className = o.GetType().Name;
            var stackTrace = new StackTrace();
            var frame = stackTrace.GetFrame(1) ?? stackTrace.GetFrame(0);
            var callerName = frame == null ? "null" : frame.GetMethod().Name;
            
            var tag = jsonParams.Length % 2 == 1 ? jsonParams[0]?.ToString() ?? "null" : string.Empty;
            if (o is ISharedLogTag sharedLogTag) tag = sharedLogTag.LogTag;
            var fixedJsonParams = jsonParams.Length % 2 == 1 ? jsonParams.Skip(1).ToArray() : jsonParams;

            var dataString = StringUtils.ToJsonString(fixedJsonParams);

            var startWith = $"{tag} {className}->{callerName}: ";
            var arr = dataString.SplitIntoChunks(800);
            foreach (var str in arr) Debug.LogError($"{startWith}{str}");
        }
        
        [Conditional("LOG_INFO")]
        public static void LogInfo(this object o, params object[] jsonParams)
        {
            var className = o?.GetType().Name ?? "NullClassName";
            var stackTrace = new StackTrace();
            var frame = stackTrace.GetFrame(1) ?? stackTrace.GetFrame(0);
            var callerName = frame == null ? "null" : frame.GetMethod().Name;
            
            var tag = string.Empty;
            if (o is ISharedLogTag sharedLogTag) tag = sharedLogTag.LogTag;
            tag = jsonParams.Length % 2 == 1 && jsonParams[0] is string ? (string)jsonParams[0] : tag;
            var fixedJsonParams = jsonParams.Length % 2 == 1 ? jsonParams.Skip(1).ToArray() : jsonParams;
            
            var dataString = StringUtils.ToJsonString(fixedJsonParams);
            
            var startWith = $"{tag} {className}->{callerName}: ";
            var arr = dataString.SplitIntoChunks(800);
            foreach (var str in arr) UnityEngine.Debug.Log($"{startWith}{str}");
        }
        
        [Conditional("LOG_INFO")]
        public static void LogWarning(this object o, params object[] jsonParams)
        {
            var className = o.GetType().Name;
            var stackTrace = new StackTrace();
            var frame = stackTrace.GetFrame(1) ?? stackTrace.GetFrame(0);
            var callerName = frame == null ? "null" : frame.GetMethod().Name;
            
            var tag = string.Empty;
            if (o is ISharedLogTag sharedLogTag) tag = sharedLogTag.LogTag;
            tag = jsonParams.Length % 2 == 1 ? jsonParams[0].ToString() : tag;
            var fixedJsonParams = jsonParams.Length % 2 == 1 ? jsonParams.Skip(1).ToArray() : jsonParams;
            
            var dataString = StringUtils.ToJsonString(fixedJsonParams);
            
            var startWith = $"{tag} {className}->{callerName}: ";
            var arr = dataString.SplitIntoChunks(800);
            foreach (var str in arr) UnityEngine.Debug.LogWarning($"{startWith}{str}");
        }
        
        [Conditional("LOG_INFO")]
        public static void LogInfoCustom(string tag, string className, string functionName, params object[] jsonParams)
        {
            var dataString = StringUtils.ToJsonString(jsonParams);
            var startWith = $"{tag} {className}->{functionName}: ";
            var arr = dataString.SplitIntoChunks(800);
            foreach (var str in arr) UnityEngine.Debug.Log($"{startWith}{str}");
        }
        
        public static void LogErrorProduction(string logMsg)
        {
#if UNITY_ANDROID
            var arr = logMsg.SplitIntoChunks(800);
            foreach (var str in arr) UnityEngine.Debug.LogError(str);
#else
            UnityEngine.Debug.LogError(logMsg);
#endif
        }
    }
}