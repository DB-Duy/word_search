using Shared.Utilities.SharedBehaviour;
using UnityEngine;

namespace Shared.Utils
{
    [DisallowMultipleComponent]
    public class PositionStickBehaviour : SharedMonoBehaviour
    {
        [SerializeField] private Transform _following;
        [SerializeField] private Transform _myTransform;

        private void Update()
        {
            if (_myTransform != null && _following != null) this._myTransform.position = _following.position;
        }

#if UNITY_EDITOR
        public override void GUIEditor()
        {
            GUILayout.Label("Editor Quick Actions");
            base.GUIEditor();

            if (GUILayout.Button("Stick"))
            {
                if (_myTransform != null && _following != null) this._myTransform.position = _following.position;
            }
        }
#endif
    }
}