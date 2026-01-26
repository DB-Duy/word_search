using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using SharedEditor.Editor.Utils;
using UnityEditor;
using UnityEngine;

namespace Game.Shared.Editor
{
    [InitializeOnLoad]
    public class GvhSettingsManager
    {
        // Đường dẫn tương đối đến file cài đặt trong dự án Unity
        private const string SettingsFilePath = "ProjectSettings/GvhProjectSettings.xml";

        static GvhSettingsManager()
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

            ApplyCustomGvhSettings();
            // Remove callback to avoid multiple calls
            EditorApplication.delayCall -= OnAfterDomainReload;
        }

        // Tạo một menu item trong Unity Editor tại "Tools/Apply Custom Gvh Settings"
        public static void ApplyCustomGvhSettings()
        {
            // Danh sách các cài đặt cần được thêm hoặc cập nhật
            var settingsToApply = new Dictionary<string, string>
            {
                { "Google.IOSResolver.AutoPodToolInstallInEditor", "True" },
                { "Google.IOSResolver.CocoapodsIntegrationMethod", "2" },
                { "Google.IOSResolver.PodfileAddUseFrameworks", "True" },
                { "Google.IOSResolver.PodfileAllowPodsInMultipleTargets", "True" },
                { "Google.IOSResolver.PodfileAlwaysAddMainTarget", "True" },
                { "Google.IOSResolver.PodfileEnabled", "True" },
                // Note: Google.IOSResolver.PodfileStaticLinkFrameworks phải set = false thì mới gắn search path chạy được nhé. 
                { "Google.IOSResolver.PodfileStaticLinkFrameworks", "False" },
                { "Google.IOSResolver.PodToolExecutionViaShellEnabled", "True" },
                { "Google.IOSResolver.PodToolShellExecutionSetLang", "True" },
                { "Google.IOSResolver.SwiftFrameworkSupportWorkaroundEnabled", "True" },
                { "Google.IOSResolver.SwiftLanguageVersion", "5.0" },
                { "Google.IOSResolver.VerboseLoggingEnabled", "False" },
                { "Google.VersionHandler.VerboseLoggingEnabled", "False" },
                { "GooglePlayServices.PromptBeforeAutoResolution", "False" },
            };

            // Lấy đường dẫn đầy đủ đến file
            var projectPath = SharedEditorUtils.GetProjectPath();
            var fullPath = Path.Combine(projectPath, SettingsFilePath);

            XDocument doc;
            XElement root;

            // Kiểm tra xem file đã tồn tại chưa
            if (File.Exists(fullPath))
            {
                // Nếu có, tải file XML
                doc = XDocument.Load(fullPath);
                root = doc.Root;
                if (root == null || root.Name != "projectSettings")
                {
                    // Nếu file bị hỏng hoặc không đúng định dạng, tạo mới
                    Debug.LogWarning("GvhProjectSettings.xml is malformed. Creating a new one.");
                    root = new XElement("projectSettings");
                    doc = new XDocument(root);
                }
            }
            else
            {
                // Nếu không, tạo một cấu trúc XML mới
                root = new XElement("projectSettings");
                doc = new XDocument(root);
            }

            var hasChanged = false;

            // Duyệt qua từng cài đặt bạn muốn áp dụng
            foreach (var setting in settingsToApply)
            {
                var settingName = setting.Key;
                var settingValue = setting.Value;

                // Tìm element <projectSetting> có thuộc tính 'name' tương ứng
                var existingSetting = root.Elements("projectSetting").FirstOrDefault(e => (string)e.Attribute("name") == settingName);

                if (existingSetting != null)
                {
                    // Nếu đã tồn tại, kiểm tra xem giá trị có cần cập nhật không
                    if ((string)existingSetting.Attribute("value") != settingValue)
                    {
                        Debug.Log($"Updating setting: {settingName} to {settingValue}");
                        existingSetting.SetAttributeValue("value", settingValue);
                        hasChanged = true;
                    }
                }
                else
                {
                    // Nếu chưa tồn tại, tạo một element mới và thêm vào root
                    Debug.Log($"Adding new setting: {settingName} with value {settingValue}");
                    root.Add(new XElement("projectSetting", new XAttribute("name", settingName), new XAttribute("value", settingValue)
                    ));
                    hasChanged = true;
                }
            }

            // Chỉ lưu lại file nếu có sự thay đổi
            if (hasChanged)
            {
                // Đảm bảo thư mục ProjectSettings tồn tại
                Directory.CreateDirectory(Path.GetDirectoryName(fullPath));
                doc.Save(fullPath);
                Debug.Log("Successfully applied custom settings to GvhProjectSettings.xml.");
            }
            else
            {
                Debug.Log("All settings in GvhProjectSettings.xml are already up to date.");
            }

            // Refresh AssetDatabase để Unity nhận biết sự thay đổi (tùy chọn)
            AssetDatabase.Refresh();
        }
        
        // Google.IOSResolver.PodfileStaticLinkFrameworks = True sẽ sinh ra lỗi.
        // Error loading /var/containers/Bundle/Application/2F21A983-9151-4FA4-A0DF-58EC3D0E2D29/Hole.app/Frameworks/UnityFramework.framework/UnityFramework (131):  dlopen(/var/containers/Bundle/Application/2F21A983-9151-4FA4-A0DF-58EC3D0E2D29/Hole.app/Frameworks/UnityFramework.framework/UnityFramework, 0x0109): Library not loaded: @rpath/ATOM.framework/ATOM
        //     Referenced from: <DF860994-4644-310A-B603-E11F750DF82D> /private/var/containers/Bundle/Application/2F21A983-9151-4FA4-A0DF-58EC3D0E2D29/Hole.app/Frameworks/UnityFramework.framework/UnityFramework
        //     Reason: tried: '/usr/lib/swift/ATOM.framework/ATOM' (no such file, not in dyld cache), '/private/preboot/Cryptexes/OS/usr/lib/swift/ATOM.framework/ATOM' (no such file), '/private/var/containers/Bundle/Application/2F21A983-9151-4FA4-A0DF-58EC3D0E2D29/Hole.app/Frameworks/ATOM.framework/ATOM' (no such file), '/usr/lib/swift/ATOM.framework/ATOM' (no such file, not in dyld cache), '/private/preboot/Cryptexes/OS/usr/lib/swift/ATOM.framework/ATOM' (no such file), '/private/var/containers/Bundle/Application/2F21A983-9151-4FA4-A0DF-58EC3D0E2D29/Hole.app/Frameworks/ATOM.framework/ATOM' (no such file), '/private/var/containers/Bundle/Application/2F21A983-9151-4FA4-A0DF-58EC3D0E2D29/Hole.app/Frameworks/ATOM.framework/ATOM' (no such file)
    }
}