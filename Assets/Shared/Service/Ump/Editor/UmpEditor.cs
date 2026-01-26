#if USING_UMP
using System;
using System.Reflection;
using SharedEditor.Editor.Utils;
using UnityEditor;
using UnityEngine;

namespace Shared.Service.Ump.Editor
{
    [InitializeOnLoad]
    public class UmpEditor
    {
        static UmpEditor()
        {
            EditorApplication.delayCall += OnAfterDomainReload;
        }

        private static void OnAfterDomainReload()
        {
            // Đảm bảo không thực thi khi đang Play hoặc sắp Play
            if (EditorApplication.isPlayingOrWillChangePlaymode)
            {
                EditorApplication.delayCall -= OnAfterDomainReload;
                return;
            }
            
            _CreateGoogleAdsSettings(); 
            // Remove callback to avoid multiple calls
            EditorApplication.delayCall -= OnAfterDomainReload;
#if ENABLE_SHARED_EDITOR_LOGGER
            Debug.Log("✅ UmpEditor Domain reload complete!");
#endif
        }

        private static void _CreateGoogleAdsSettings()
        {
            var type = Type.GetType("GoogleMobileAds.Editor.GoogleMobileAdsSettings, GoogleMobileAds.Editor");

            if (type == null)
            {
                Debug.LogError("Could not find type.");
                return;
            }

            // Call static method LoadInstance
            var loadInstanceMethod = type.GetMethod("LoadInstance", BindingFlags.NonPublic | BindingFlags.Static);
            var instance = loadInstanceMethod?.Invoke(null, null);

            if (instance == null)
            {
                Debug.LogError("Failed to load GoogleMobileAdsSettings instance.");
                return;
            }
            
            
            var androidKey = SharedEditorUtils.GetKeyConfigValue("admob_app_id", "android");

            // Get a property value
            var androidAppIdProp = type.GetProperty("GoogleMobileAdsAndroidAppId", BindingFlags.Public | BindingFlags.Instance);
            androidAppIdProp?.SetValue(instance, androidKey);
            
            var iosKey = SharedEditorUtils.GetKeyConfigValue("admob_app_id", "ios");
            if (!string.IsNullOrEmpty(iosKey))
            {
                var iosAppIdProp = type.GetProperty("GoogleMobileAdsIOSAppId", BindingFlags.Public | BindingFlags.Instance);
                iosAppIdProp?.SetValue(instance, iosKey);
            }
            
            // Mark dirty and save
            EditorUtility.SetDirty((ScriptableObject)instance);
            AssetDatabase.SaveAssets();
        }
    }
}
#endif