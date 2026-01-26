#if FACEBOOK_SDK
using Facebook.Unity.Settings;
using SharedEditor.Editor.Utils;
using UnityEditor;
using UnityEngine;

namespace Shared.Service.Facebook.Editor
{
    [InitializeOnLoad]
    public class FacebookEditor
    {
        static FacebookEditor()
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
            
            CreateFacebookSettings(); 
            // Remove callback to avoid multiple calls
            EditorApplication.delayCall -= OnAfterDomainReload;
#if ENABLE_SHARED_EDITOR_LOGGER
            Debug.Log("✅ FacebookEditor Domain reload complete!");
#endif
        }

        private static void CreateFacebookSettings()
        {
            var clientToken = SharedEditorUtils.GetKeyConfigValue("facebook_client_token", "android");
            var facebookId = SharedEditorUtils.GetKeyConfigValue("facebook_id", "android");
            var appName = SharedEditorUtils.GetBuildConfigValue("name", "android");
            var androidKeySign = SharedEditorUtils.GetBuildConfigValue("android_keystore", "android");
            
            FacebookSettings.ClientTokens.Clear();
            FacebookSettings.ClientTokens.Add(clientToken);
            
            FacebookSettings.AppIds.Clear();
            FacebookSettings.AppIds.Add(facebookId);
            
            FacebookSettings.AppLabels.Clear();
            FacebookSettings.AppLabels.Add(appName);

            FacebookSettings.SelectedAppIndex = 0;
            FacebookSettings.Cookie = true;
            FacebookSettings.Logging = true;
            FacebookSettings.Status = true;
            FacebookSettings.Xfbml = false;
            FacebookSettings.FrictionlessRequests = true;
            FacebookSettings.AndroidKeystorePath = androidKeySign;
            FacebookSettings.AutoLogAppEventsEnabled = true;
            FacebookSettings.AdvertiserIDCollectionEnabled = true;
            
            // FacebookSettings.Instance.
    
            SharedEditorUtils.CreateDirectoriesFromRootByAssetsRelativePath(FacebookSettings.FacebookSettingsPath);
            var path = $"Assets/{FacebookSettings.FacebookSettingsPath}/{FacebookSettings.FacebookSettingsAssetName}{FacebookSettings.FacebookSettingsAssetExtension}";

            var check = AssetDatabase.LoadAssetAtPath<FacebookSettings>(path);
            if (check == null)
            {
                AssetDatabase.CreateAsset(FacebookSettings.Instance, path);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }
            else
            {
                EditorUtility.SetDirty(FacebookSettings.Instance); // Mark it as dirty so Unity saves it
                AssetDatabase.SaveAssets();   // Ensure changes are saved to disk    
            }   
        }
    }
}

#endif