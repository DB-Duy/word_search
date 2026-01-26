using Shared.Core.Async;
using Shared.Core.IoC;
using Shared.Core.Repository.Prefab;
using Shared.Core.View.Common;
using Shared.Core.View.Dialog;
using Shared.Core.View.FloatingMessage;
using Shared.Core.View.Loading;
using Shared.Core.View.Screen;
using UnityEngine;

namespace Shared.Core.View.Scene
{
    public static class UISceneExtensions
    {
        private static UIScene Scene => UIScene.Instance ? UIScene.Instance : Object.FindObjectOfType<UIScene>();

        // -------------------------------------------------------------------------------------------------------------
        // Screen
        // -------------------------------------------------------------------------------------------------------------
        public static void ShowScreen<T>(this ISharedUtility u) where T : MonoBehaviour, IUIScreen => Scene.ShowScreen<T>();
        public static void DestroyInactiveScreen<T>(this ISharedUtility u) => Scene.DestroyInactiveScreen<T>();
        public static void DestroyAllInactiveScreen(this ISharedUtility u) => Scene.DestroyAllInactiveScreen();
        
        // Dialog
        public static IAsyncOperation<T> ShowDialog<T>(this ISharedUtility u, DialogAction dialogAction = DialogAction.HideLastOne) where T : MonoBehaviour, IUIDialog => Scene.ShowDialog<T>(dialogAction);
        public static T HideDialog<T>(this ISharedUtility u) where T : MonoBehaviour, IUIDialog => Scene.HideDialog<T>();
        public static void HideDialog(this ISharedUtility u, IUIDialog dialog) => Scene.HideDialog(dialog);
        public static void HideCurrentDialog(this ISharedUtility u) => Scene.HideCurrentDialog();
        public static void HideAllDialog(this ISharedUtility u, bool ignoreCurrent = true) => Scene.HideAllDialog();
        public static void DestroyDialog<T>(this ISharedUtility u) where T : MonoBehaviour, IUIDialog => Scene.DestroyDialog<T>();
        public static void DestroyDialog(this ISharedUtility u, IUIDialog dialog) => Scene.DestroyDialog(dialog);
        
        // Floating Messages
        public static void ShowFloatingMessage<T>(this ISharedUtility u, string m) where T : MonoBehaviour, IUIFloatingMessage => Scene.ShowFloatingMessage<T>(m);
        
        // Loading Indicator
        public static void ShowActivityIndicator<T>(this ISharedUtility u, string id) where T : MonoBehaviour, IUIActivityIndicator => Scene.ShowActivityIndicator<T>(id);
        public static void HideActivityIndicator(this ISharedUtility u, string id) => Scene.HideActivityIndicator(id);
        
        public static T InstantiateSharedFeature<T>(this ISharedUtility api, Transform parent) where T : MonoBehaviour
        {
            var r = IoCExtensions.Instance.Resolve<IPrefabRepository>();
            var prefab = r.GetOrLoad<T>();
            return Object.Instantiate(prefab, parent);
        }
        
        
    }
}