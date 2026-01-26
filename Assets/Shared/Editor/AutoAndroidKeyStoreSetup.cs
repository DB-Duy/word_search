using System;
using System.IO;
using SharedEditor.Editor.Utils;
using UnityEditor;
using UnityEngine;

namespace Game.Shared.Editor
{
    [InitializeOnLoad]
    public class AutoAndroidKeyStoreSetup
    {
        
        static AutoAndroidKeyStoreSetup()
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
            
            var org = SharedEditorUtils.GetBuildConfigValue("org", "any");
            var home = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            var sourcePath = Path.Combine(home, $".{org}", "keys");
            
            var projectPath = Path.GetFullPath(Path.Combine(Application.dataPath, ".."));
            var targetPath = Path.Combine(projectPath, "keys");
            
            var companyLogoSourceFile = Path.Combine(home, $".{org}", "company_logo.png");
            var companyLogoTargetFolder = Path.Combine(Application.dataPath, "AAA");
            var companyLogoTargetFile = Path.Combine(companyLogoTargetFolder,  "company_logo.png");
            if (File.Exists(companyLogoSourceFile))
            {
                if (!Directory.Exists(companyLogoTargetFolder)) 
                    Directory.CreateDirectory(companyLogoTargetFolder);
                File.Copy(companyLogoSourceFile, companyLogoTargetFile, true);
            }

            if (!Directory.Exists(targetPath) && Directory.Exists(sourcePath))
            {
                SharedEditorUtils.CopyDirectory(sourcePath, targetPath, overwrite: true);
            }
            
            if (EditorUserBuildSettings.activeBuildTarget == BuildTarget.Android)
            {
                var (keystore, keystorePassword, alias, aliasPassword) = SharedEditorUtils.GetAndroidKeyStoreConfig();
                if (string.IsNullOrEmpty(keystore))
                {
                    PlayerSettings.Android.useCustomKeystore = true;
                    PlayerSettings.Android.keystoreName = keystore;
                    PlayerSettings.Android.keystorePass = keystorePassword;
                    PlayerSettings.Android.keyaliasName = alias;
                    PlayerSettings.Android.keyaliasPass = aliasPassword;    
                }
            }

            EditorApplication.delayCall -= OnAfterDomainReload;
#if ENABLE_SHARED_EDITOR_LOGGER
            Debug.Log("✅ AutoAndroidKeyStoreSetup Domain reload complete!");
#endif
        }
    }
}