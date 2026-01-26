using System;
using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.TestTools;

namespace Shared.SharedReport.Editor
{
    public class TestCases
    {
        
        [UnityTest]
        public IEnumerator TestResponse()
        {
            var jsonData = "{\"versionName\":\"2.4.3\",\"versionCode\":\"241008327\",\"packageName\":\"com.indiez.nonogram\",\"versions\":{\"applovin\":{\"android_adapter\":\"4.3.47\",\"android_native\":\"13.0.0\"},\"aps\":{\"android_adapter\":\"4.3.14\",\"android_native\":\"aps-android-9.10.2\"},\"bid_machine\":{\"android_adapter\":\"4.3.9\",\"android_native\":\"3.0.1\"},\"bigo\":{\"android_adapter\":\"4.3.1\",\"android_native\":\"4.9.1\"},\"chartboost\":{\"android_adapter\":\"4.3.16\",\"android_native\":\"9.7.0\"},\"digital_turbine\":{\"android_adapter\":\"4.3.33\",\"android_native\":\"8.3.1\"},\"admob\":{\"android_adapter\":\"4.3.45\",\"android_native\":\"23.3.0\"}}}";
            var url = "http://localhost:8080/app/android/version/verify";
            // var url = "https://s2s.indiez.net/app/android/version/verify";
            using var www = UnityWebRequest.Post(url, jsonData, "application/json");
            www.SetRequestHeader("x-api-key", "aeee2cea-5b1b-452d-84e5-bb984341596e");
            
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                
                Debug.Log($"TestResponse: www.result={www.result} {www.error}");
            }
            else
            {
                var responseString = www.downloadHandler.text;
                Debug.Log($"TestResponse: {responseString}");
            }
            yield return null;
        }
        [UnityTest]
        public IEnumerator TestAccessPoint()
        {
            var accessPoint = "com.ironsource.adapters.aps.BuildConfig.VERSION_NAME";
            var lastIndexOfDot = accessPoint.LastIndexOf(".", StringComparison.Ordinal) + 1;
            var variableOrFunctionName = accessPoint[lastIndexOfDot..];
            var className = accessPoint.Replace($".{variableOrFunctionName}", string.Empty);
            var isFunction = variableOrFunctionName.EndsWith("()");
            Assert.AreEqual("VERSION_NAME", variableOrFunctionName);
            Assert.AreEqual("com.ironsource.adapters.aps.BuildConfig", className);
            yield return null;
        }
    }
}