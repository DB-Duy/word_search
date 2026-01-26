#if UNITY_IOS && APPS_FLYER
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.iOS.Xcode;
using System.IO;
using UnityEngine;

namespace Shared.Service.SharedAppsFlyer.Editor
{
    public class PostProcessInfoPlist
    {
        [PostProcessBuild]
        public static void OnPostProcessBuild(BuildTarget buildTarget, string pathToBuiltProject)
        {
            if (buildTarget != BuildTarget.iOS) return;

            // Path to Info.plist
            var plistPath = Path.Combine(pathToBuiltProject, "Info.plist");

            // Read Info.plist
            var plist = new PlistDocument();
            plist.ReadFromFile(plistPath);

            // Set NSAdvertisingAttributionReportEndpoint
            var rootDict = plist.root;
            rootDict.SetString("NSAdvertisingAttributionReportEndpoint", "https://appsflyer-skadnetwork.com/");

            // Write changes back to Info.plist
            File.WriteAllText(plistPath, plist.WriteToString());
            Debug.Log("✅ NSAdvertisingAttributionReportEndpoint added to Info.plist");
        }
    }
}

#endif