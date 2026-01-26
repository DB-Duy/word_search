using SharedEditor.Editor.Utils;
using UnityEditor;
using UnityEngine;

namespace Shared.Editor
{
    [InitializeOnLoad]
    public class BuildGatewayShellBuilder
    {
        static BuildGatewayShellBuilder()
        {
            EditorApplication.delayCall += OnAfterDomainReload;
        }

        private static void OnAfterDomainReload()
        {
            if (EditorApplication.isPlayingOrWillChangePlaymode)
            {
                EditorApplication.delayCall -= OnAfterDomainReload;
                return;
            }
            var editorVersion = SharedEditorUtils.GetBuildConfigValue("editor_version", "any");
            editorVersion = string.IsNullOrEmpty(editorVersion) ? Application.unityVersion : editorVersion;
            
            Build("gateway_ios_appstore_development", $"/Applications/Unity/Hub/Editor/{editorVersion}/Unity.app/Contents/MacOS/Unity", "iOS", "SharedEditor.Editor.GameBuilder.PerformFirebaseAppDistributionIpa");
            Build("gateway_ios_appstore", $"/Applications/Unity/Hub/Editor/{editorVersion}/Unity.app/Contents/MacOS/Unity", "iOS", "SharedEditor.Editor.GameBuilder.PerformIOSBuildForAppStore");
            
            Build("gateway_ios_china_appstore_development", $"/Applications/Unity/Hub/Editor/{editorVersion}/Unity.app/Contents/MacOS/Unity", "iOS", "SharedEditor.Editor.GameBuilder.PerformFirebaseAppDistributionIpaChina");
            Build("gateway_ios_china_appstore", $"/Applications/Unity/Hub/Editor/{editorVersion}/Unity.app/Contents/MacOS/Unity", "iOS", "SharedEditor.Editor.GameBuilder.PerformIOSBuildForAppStoreChina");
            
            Build("gateway_android_googleplay_development", $"/Applications/Unity/Hub/Editor/{editorVersion}/Unity.app/Contents/MacOS/Unity", "Android", "SharedEditor.Editor.GameBuilder.PerformFirebaseAppDistributionApk");
            Build("gateway_android_googleplay", $"/Applications/Unity/Hub/Editor/{editorVersion}/Unity.app/Contents/MacOS/Unity", "Android", "SharedEditor.Editor.GameBuilder.PerformAndroidBuildForGooglePlay");
            
            Build("gateway_android_huawei_development", $"/Applications/Unity/Hub/Editor/{editorVersion}/Unity.app/Contents/MacOS/Unity", "Android", "SharedEditor.Editor.GameBuilder.PerformAndroidHuaweiDistributionDevelopment");
            Build("gateway_android_huawei", $"/Applications/Unity/Hub/Editor/{editorVersion}/Unity.app/Contents/MacOS/Unity", "Android", "SharedEditor.Editor.GameBuilder.PerformAndroidHuaweiDistribution");
            
            EditorApplication.delayCall -= OnAfterDomainReload;
#if ENABLE_SHARED_EDITOR_LOGGER
            Debug.Log("✅ BuildGatewayShellBuilder Domain reload complete!");
#endif
        }
        
        private static void Build(string projectRelativeFilePath, string editorPath, string buildTarget, string method)
        {
            // Application.unityVersion
            var content = $@"
cd ""$(dirname ""$0"")""
PROJECT_PATH=""$PWD""
pwd
rm -rf Logs/build.log
{editorPath} -quit -batchmode -buildTarget {buildTarget} -projectPath ""$PROJECT_PATH"" -executeMethod {method} -logFile Logs/build.log
exit 0
";
            _ = SharedEditorUtils.WriteProjectRelativeFile(projectRelativeFilePath, content, silent: true);
        }
    }
}