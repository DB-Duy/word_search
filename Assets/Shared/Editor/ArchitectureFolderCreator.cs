using System.Collections.Generic;
using SharedEditor.Editor.Utils;
using UnityEditor;

namespace Game.Shared.Editor
{
    [InitializeOnLoad]
    public class ArchitectureFolderCreator
    {
        static ArchitectureFolderCreator()
        {
            EditorApplication.delayCall += OnAfterDomainReload;
        }

        private static void OnAfterDomainReload()
        {
            // Assets/Scripts/Service
            SharedEditorUtils.CreateProjectRelativeFolders(new List<string>
            {
                "Assets/Scripts/Entity",
                "Assets/Scripts/Repository",
                "Assets/Scripts/Service",
                "Assets/Scripts/View"
            });
#if ENABLE_SHARED_EDITOR_LOGGER
            Debug.Log("✅ ArchitectureFolderCreator Domain reload complete!");
#endif
        }
    }
}