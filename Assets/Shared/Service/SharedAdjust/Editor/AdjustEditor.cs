#if ADJUST
using SharedEditor.Editor.Utils;
using UnityEditor;
using UnityEngine;

namespace Shared.Service.SharedAdjust.Editor
{
    [InitializeOnLoad]
    public class AdjustEditor
    {
        static AdjustEditor()
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
            Debug.Log("✅ AdjustEditor Domain reload complete!");
            CreateAdjustSettings(); 
            // Remove callback to avoid multiple calls
            EditorApplication.delayCall -= OnAfterDomainReload;
        }
        
        public static void CreateAdjustSettings()
        {
            SharedEditorUtils.CreateDirectoriesFromRootByAssetsRelativePath("Adjust/Resources");
            var path = "Assets/Adjust/Resources/AdjustSettings.asset";
            var settings = ScriptableObject.CreateInstance<AdjustSdk.AdjustSettings>();
            AssetDatabase.CreateAsset(settings, path);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log("AdjustSettings.asset created at: " + path);
        }
        
        // private static void SetField<T>(AdjustSettings obj, string fieldName, T value)
        // {
        //     var field = typeof(AdjustSettings).GetField(fieldName, System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        //     if (field != null)
        //     {
        //         field.SetValue(obj, value);
        //     }
        //     else
        //     {
        //         Debug.LogWarning("⚠️ Field not found: " + fieldName);
        //     }
        // }
    }
}
#endif