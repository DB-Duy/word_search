using Shared.Utilities.SharedBehaviour;
using UnityEngine;
using UnityEngine.UI;

namespace Shared.View.StoreRating 
{
    [DisallowMultipleComponent]
    public class StoreRatingStar : SharedMonoBehaviour
    {
        private static readonly int IsLit = Animator.StringToHash("isLit");
        
        [SerializeField] private Button button;
        [SerializeField] private Animator animator;

        public Button Button => button;

        public void SetLightState(bool isLit)
        {
            animator.SetBool(IsLit, isLit);
        }

        public void LightUp() => SetLightState(true);
        public void LightOff() => SetLightState(false);

#if UNITY_EDITOR
        public override void GUIEditor()
        {
            GUILayout.Label("Editor Quick Actions");
            base.GUIEditor();

            if (GUILayout.Button("LightUp"))
            {
                LightUp();
            }
            if (GUILayout.Button("LightOff"))
            {
                LightOff();
            }
        }
#endif
    }
}
