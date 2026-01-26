#if ZENJECT
using UnityEditor;
using UnityEngine;
using Zenject;

namespace Game.Shared.Editor
{
    public class SceneTemplateSetup
    {
        [MenuItem("Shared/Template/Setup Scene")]
        private static void SetupSceneContextFromPrefab()
        {
            var sceneContext = Object.FindObjectOfType<SceneContext>();
            if (sceneContext == null)
            {
                var go = new GameObject("SceneContext");
                go.AddComponent<SceneContext>();

                Undo.RegisterCreatedObjectUndo(go, "Create SceneContext");
                Debug.Log("[Zenject] SceneContext created in current scene.");
            }
            else
            {
                Debug.LogWarning("[Zenject] SceneContext already exists in this scene.");
            }
        }
    }
}
#endif