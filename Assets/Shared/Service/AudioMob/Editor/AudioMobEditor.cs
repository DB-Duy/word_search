#if AUDIO_MOB
using Audiomob;
using SharedEditor.Editor.Utils;
using UnityEditor;
using UnityEngine;

namespace Shared.Service.AudioMob.Editor
{
    [InitializeOnLoad]
    public class AudioMobEditor
    {
        static AudioMobEditor()
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
            Debug.Log("✅ AudioMobEditor Domain reload complete!");
            const string settingsPath = "Assets/Plugins/AudioMob/Resources/Settings/audiomob-settings.asset";
            // Tải ScriptableObject
            // Thay thế 'AudioMobSettings' bằng tên lớp thực tế của file settings
            var settings = AssetDatabase.LoadAssetAtPath<AudiomobSettings>(settingsPath);
#if GOOGLE_PLAY
            settings.ApiKey = SharedEditorUtils.GetKeyConfigValue("audiomob_id", "android");
#elif APPSTORE
            settings.ApiKey = SharedEditorUtils.GetKeyConfigValue("audiomob_id", "ios");
#endif
            EditorUtility.SetDirty(settings);
            // Lưu các thay đổi vào đĩa
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh(); // Làm mới AssetDatabase để thay đổi được nhận diện
            
            // Remove callback to avoid multiple calls
            EditorApplication.delayCall -= OnAfterDomainReload;
        }
    }
}
#endif