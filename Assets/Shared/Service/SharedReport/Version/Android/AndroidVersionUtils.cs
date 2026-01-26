using System;
using Shared.Utils;
using UnityEngine;

namespace Shared.SharedReport.Version.Android
{
    public static class AndroidVersionUtils
    {
        private const string TAG = "AndroidVersionUtils";
        public static string GetVersionViaStaticFunction(string className, string staticFunctionName)
        {
            try
            {
                var javaClass = new AndroidJavaClass(className);
                var version = javaClass.CallStatic<string>(staticFunctionName);
                return CleanUpVersionString(version);
            }
            catch (Exception e)
            {
                SharedLogger.Log($"{TAG}->GetVersionViaStaticFunction: {e.Message}");
                return "null";
            }
        }
        
        public static string GetVersionViaStaticField(string className, string staticFieldName)
        {
            try
            {
                var javaClass = new AndroidJavaClass(className);
                return javaClass.GetStatic<string>(staticFieldName);
            }
            catch (Exception e)
            {
                SharedLogger.Log($"{TAG}->GetVersionViaStaticField: {e.Message}");
                return "null";
            }
        }
        
        public static string GetVersion(AndroidConstants.StaticJavaFunction define)
        {
            return define.IsFunction ? GetVersionViaStaticFunction(define.ClassName, define.FunctionName) : GetVersionViaStaticField(define.ClassName, define.FunctionName);
        }
        
        // com.ironsource.adapters.superawesome.BuildConfig.VERSION_NAME
        public static string GetVersionViaAccessPoint(string accessPoint)
        {
            if (string.IsNullOrEmpty(accessPoint) || accessPoint.StartsWith("ignore://")) return "null";
            var lastIndexOfDot = accessPoint.LastIndexOf(".", StringComparison.Ordinal) + 1;
            var variableOrFunctionName = accessPoint[lastIndexOfDot..];
            var className = accessPoint.Replace($".{variableOrFunctionName}", string.Empty);
            var isFunction = variableOrFunctionName.EndsWith("()");
            if (isFunction) variableOrFunctionName = variableOrFunctionName.Replace("()", string.Empty);
            return isFunction ? GetVersionViaStaticFunction(className, variableOrFunctionName) : GetVersionViaStaticField(className, variableOrFunctionName);
        }

        public static string CleanUpVersionString(string version)
        {
            var result = version;
            // "aps": "android_native": "aps-android-9.10.2",
            result = result.Replace("aps-android-", string.Empty);
            // "mintegral": "android_native": "MAL_16.8.51",
            result = result.Replace("MAL_", string.Empty);
            // "verve": "android_native": "sdkandroid_b_3.0.4",
            result = result.Replace("sdkandroid_b_", string.Empty);
            return result;
        }
    }
}