#if FIREBASE && LOG_INFO && UNITY_IOS && ENABLE_FIREBASE_DEBUG
using UnityEditor;
using UnityEditor.Callbacks;
using System.IO;
using System.Text.RegularExpressions;

namespace Shared.Service.Firebase.Editor
{
    public class FirebasePostProcessBuild
    {
        
        [PostProcessBuild(999)]
        public static void OnPostprocessBuild(BuildTarget target, string pathToBuiltProject)
        {
            if (target != BuildTarget.iOS) return;

            var filePath = Path.Combine(pathToBuiltProject, "Classes/UnityAppController.mm");

            if (!File.Exists(filePath))
            {
                UnityEngine.Debug.LogWarning("Không tìm thấy UnityAppController.mm");
                return;
            }

            var content = File.ReadAllText(filePath);

            // Check if already injected
            if (content.Contains("setValue:[newArguments copy] forKey:@\"arguments\""))
            {
                UnityEngine.Debug.Log("✅ FIRDebugEnabled đã được chèn sẵn vào UnityAppController.mm");
                return;
            }

            // Inject 3 dòng sau printf("-> applicationDidFinishLaunching()\n");
            var pattern = @"::printf\(""-> applicationDidFinishLaunching\(\)\\n""\);";
            var injection = @"::printf(""-> applicationDidFinishLaunching()\n"");

    NSMutableArray *newArguments = [NSMutableArray arrayWithArray:[[NSProcessInfo processInfo] arguments]];
    [newArguments addObject:@""-FIRDebugEnabled""];
    [[NSProcessInfo processInfo] setValue:[newArguments copy] forKey:@""arguments""];";

            var updatedContent = Regex.Replace(content, pattern, injection, RegexOptions.IgnoreCase);

            File.WriteAllText(filePath, updatedContent);
            UnityEngine.Debug.Log("✅ Đã chèn -FIRDebugEnabled vào didFinishLaunchingWithOptions trong UnityAppController.mm");
        }
    }
}
#endif