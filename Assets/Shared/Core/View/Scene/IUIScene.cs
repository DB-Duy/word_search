using Shared.Core.Async;
using Shared.Core.View.Common;
using Shared.Core.View.Dialog;
using Shared.Core.View.FloatingMessage;
using Shared.Core.View.Halo;
using Shared.Core.View.Loading;
using Shared.Core.View.Screen;
using UnityEngine;

namespace Shared.Core.View.Scene
{
    public interface IUIScene
    {
        // Screen
        void ShowScreen<T>() where T : MonoBehaviour, IUIScreen;
        void DestroyInactiveScreen<T>();
        void DestroyAllInactiveScreen();
        
        // Dialog
        IAsyncOperation<T> ShowDialog<T>(DialogAction dialogAction = DialogAction.None) where T : MonoBehaviour, IUIDialog;
        T HideDialog<T>() where T : MonoBehaviour, IUIDialog;
        void HideDialog(IUIDialog dialog);
        void HideCurrentDialog();
        void HideAllDialog(bool ignoreCurrent = true);
        void DestroyDialog<T>() where T : MonoBehaviour, IUIDialog;
        void DestroyDialog(IUIDialog dialog);
        
        // Floating Messages
        void ShowFloatingMessage<T>(string m) where T : MonoBehaviour, IUIFloatingMessage;

        // Loading Indicator
        void ShowActivityIndicator<T>(string id) where T : MonoBehaviour, IUIActivityIndicator;
        void HideActivityIndicator(string id);
        
        // BackKey Handler
        void AddBackKeyHandler(IUIBackKeyHandler handler);
        void RemoveBackKeyHandler(IUIBackKeyHandler handler);

        // Halo
        T ShowHalo<T>(GameObject target) where T : MonoBehaviour, IUIHalo;
        void HideHalo(MonoBehaviour monoBehaviour);
    }
}