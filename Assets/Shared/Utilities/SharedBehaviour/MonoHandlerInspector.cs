#if UNITY_EDITOR

using UnityEditor;

namespace Shared.Utilities.SharedBehaviour
{
    [CustomEditor(typeof(SharedMonoBehaviour), true)]
    public class MonoHandlerInspector : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            var mono = (SharedMonoBehaviour)target;
            mono.GUIEditor();
        }
    }
}
#endif