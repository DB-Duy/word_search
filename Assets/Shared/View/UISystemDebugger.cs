using Shared.Core.IoC;
using Shared.Core.View.Common;
using Shared.Core.View.Scene;
using Shared.Repository.UIExample;
using Shared.Utilities.SharedBehaviour;
using Shared.View.UIExample;
using UnityEngine;
using Zenject;

namespace Shared.View
{
    public class UISystemDebugger : IoCMonoBehavior, ISharedUtility
    {
        [Inject] private UIExampleIntRepository _intRepository;
        private UIExampleDialog _dialog;
#if UNITY_EDITOR
        public override void GUIEditor()
        {
            GUILayout.Label("Editor Quick Actions");
            base.GUIEditor();
            
            GUILayout.Label("Screen");
            if (GUILayout.Button("Show"))
                this.ShowScreen<UIExampleScreen>();
            if (GUILayout.Button("IncreaseInt")) _intRepository.AddMore(1);
            if (GUILayout.Button("DecreaseInt")) _intRepository.Minus(1);
            
            GUILayout.Label("Dialog");
            if (GUILayout.Button("Show"))
                _dialog = this.ShowDialog<UIExampleDialog>().Data;
            if (GUILayout.Button("Hide") && _dialog != null)
                this.HideDialog(_dialog);

            GUILayout.Label("Floating Message");
            if (GUILayout.Button("No Internet"))
            {
                this.ShowFloatingMessage<FloatingMessage.FloatingMessage>("Oops! Please check your internet connection.");
            }
            
            GUILayout.Label("Activity Indicator");
            if (GUILayout.Button("Show"))
                this.ShowActivityIndicator<UIActivityIndicator.UIActivityIndicator>("truongps");
            if (GUILayout.Button("Hide"))
                this.HideActivityIndicator("truongps");
            
            
        }
#endif
    }
}