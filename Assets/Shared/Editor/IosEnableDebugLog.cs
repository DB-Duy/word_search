// #if UNITY_IOS || LOG_INFO
// using UnityEditor;
// using UnityEditor.Callbacks;
// using UnityEditor.iOS.Xcode;
// using UnityEngine;
//
// namespace Shared.Editor
// {
//     public class IosEnableDebugLog
//     {
//         [PostProcessBuild(999)]
//         public static void OnPostProcessBuild(BuildTarget buildTarget, string path)
//         {
//             if (buildTarget != BuildTarget.iOS)
//                 return;
//
//             var projPath = PBXProject.GetPBXProjectPath(path);
//             var proj = new PBXProject();
//             proj.ReadFromFile(projPath);
//
//             var target = proj.GetUnityMainTargetGuid();
//             var frameworkTarget = proj.GetUnityFrameworkTargetGuid();
//             
//             // Giữ lại log: Không tối ưu hóa Swift
//             proj.SetBuildProperty(target, "SWIFT_OPTIMIZATION_LEVEL", "-Onone");
//             proj.SetBuildProperty(target, "ENABLE_NS_ASSERTIONS", "YES");
//
//             // Tắt Bitcode
//             proj.SetBuildProperty(target, "ENABLE_BITCODE", "NO");
//
//             // Không strip symbols Swift
//             proj.SetBuildProperty(target, "STRIP_SWIFT_SYMBOLS", "NO");
//             
//             // Support #if DEBUG trong xcode.
//             // Get existing Swift flags (if any)
//             var flags = proj.GetBuildPropertyForAnyConfig(target, "OTHER_SWIFT_FLAGS") ?? "";
//             // Append -DDEBUG if not already present
//             if (!flags.Contains("-DDEBUG"))
//             {
//                 flags += " -DDEBUG";
//                 proj.SetBuildProperty(target, "OTHER_SWIFT_FLAGS", flags);
//                 Debug.Log("✅ Added -DDEBUG to OTHER_SWIFT_FLAGS for iOS build.");
//             }
//             else
//             {
//                 Debug.Log("ℹ️ -DDEBUG already present in OTHER_SWIFT_FLAGS.");
//             }
//
//             // Save lại Xcode project
//             proj.WriteToFile(projPath);
//
//             Debug.Log("✅ Xcode project IosEnableDebugLog auto-config completed!");
//         }
//     }
// }
// #endif