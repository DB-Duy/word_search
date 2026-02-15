using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Shared.Core.Async;
using Shared.Core.IoC;
using Shared.Core.Presenter;
using Shared.Core.Repository.Prefab;
using Shared.Core.View.Common;
using Shared.Core.View.Dialog;
using Shared.Core.View.FloatingMessage;
using Shared.Core.View.Halo;
using Shared.Core.View.Loading;
using Shared.Core.View.Screen;
using Shared.Utils;
using Shared.View.FloatingMessage;
using UnityEngine;
using Zenject;

namespace Shared.Core.View.Scene
{
    [DisallowMultipleComponent]
    public class UIScene : IoCMonoBehavior, IUIScene, IUIBackKeyHandler, ISharedUtility
    {
        private const string Tag = "UIScene";

        // public IPresenter<MonoBehaviour> DialogShownPresenter { get; } = new DialogShowPresenter();
        // public IPresenter<MonoBehaviour> DialogHidePresenter { get; } = new DialogHidePresenter();
        // public IPresenter<MonoBehaviour, MonoBehaviour> DialogChangedPresenter { get; } = null;
        // public IPresenter<MonoBehaviour> LoadingIndicatorPresenter { get; } = null;
        // public IPresenter<MonoBehaviour> FloatingMessagePresenter { get; } = new FloatingMessagePresenter();

        [Inject] private UIFloatingMessagePool _uiFloatingMessagePool;
        [Inject] private UIActivityIndicatorPool _uiActivityIndicatorPool;

        [Header("Screen Sector")] [SerializeField]
        private RectTransform screenParent;

        [Inject] private SharedFeaturePrefabRepository _prefabRepository;
        private readonly Dictionary<string, IUIScreen> _singletonScreensCache = new();
        private IUIScreen _currentScreen;

        [Inject(Optional = true)] private IScreenChangePresenter _screenChangePresenter;
        private IPresenter<MonoBehaviour, MonoBehaviour> ScreenChangePresenter => _screenChangePresenter as IPresenter<MonoBehaviour, MonoBehaviour>;

        [Header("Dialog Sector")] [SerializeField]
        private RectTransform dialogParent;

        private readonly Dictionary<string, IUIDialog> _dialogCache = new();
        private readonly List<IUIDialog> _dialogStack = new();
        private IUIDialog CurrentDialog => _dialogStack.Count > 0 ? _dialogStack[0] : null;

        [Inject(Optional = true)] private IDialogShowPresenter _dialogShownPresenter;
        private IPresenter<MonoBehaviour> DialogShownPresenter => _dialogShownPresenter ??= new DialogShowPresenter();

        [Inject(Optional = true)] private IDialogHidePresenter _dialogHidePresenter;
        private IPresenter<MonoBehaviour> DialogHidePresenter => _dialogHidePresenter ??= new DialogHidePresenter();

        [Inject(Optional = true)] private IDialogChangePresenter _dialogChangePresenter;
        private IPresenter<MonoBehaviour, MonoBehaviour> DialogChangedPresenter => _dialogChangePresenter;

        private readonly Dictionary<IUIDialog, IAsyncOperation> _dialogOperations = new();

        [Header("Loading Sector")] [SerializeField]
        private RectTransform loadingParent;

        private readonly Dictionary<string, MonoBehaviour> _loadingIndicatorCache = new();
        [Inject(Optional = true)] private IUILoadingIndicatorPresenter _loadingIndicatorPresenter;
        private IPresenter<MonoBehaviour> LoadingIndicatorPresenter => _loadingIndicatorPresenter;

        [Header("Floating Message Sector")] [SerializeField]
        private RectTransform floatingMessageParent;

        [Inject(Optional = true)] private IFloatingMessagePresenter _floatingMessagePresenter;

        private IPresenter<MonoBehaviour> FloatingMessagePresenter
        {
            get { return _floatingMessagePresenter ??= new FloatingMessagePresenter(); }
        }

        [Header("Halo Sector")] [SerializeField]
        private RectTransform haloParent;

        private readonly Dictionary<string, MonoBehaviour> _haloCache = new();

        // BackKey Handler
        private readonly HashSet<IUIBackKeyHandler> _backKeyHandlers = new();
        
        public static UIScene Instance { get; private set; }

        protected override void Awake()
        {
            base.Awake();
            Instance = this;
        }

        // -------------------------------------------------------------------------------------------------------------
        // Screen
        // -------------------------------------------------------------------------------------------------------------
        public void ShowScreen<T>() where T : MonoBehaviour, IUIScreen
        {
            var oldScreen = _currentScreen;
            var fullName = typeof(T).FullName;
            var newScreen = (T)_singletonScreensCache.Get(fullName, null);
            if (newScreen == null)
            {
                var prefab = _prefabRepository.GetOrLoad<T>();
                newScreen = Instantiate(prefab, screenParent);
                if (newScreen is ICachable)
                    _singletonScreensCache.Upsert(fullName, newScreen);
            }

            StartCoroutine(_PresentScreen((MonoBehaviour)oldScreen, newScreen));
            if (newScreen is IUIBackKeyHandler handler) AddBackKeyHandler(handler);
        }

        private IEnumerator _PresentScreen(MonoBehaviour oldScreen, MonoBehaviour newScreen)
        {
            newScreen.gameObject.SetActive(true);
            _currentScreen = newScreen as IUIScreen;
            var presenter = ScreenChangePresenter;
            if (presenter != null) yield return presenter.Present(oldScreen, newScreen);
            if (oldScreen == null) yield break;
            if (oldScreen is IUIBackKeyHandler handler) RemoveBackKeyHandler(handler);
            if (oldScreen is ICachable)
                oldScreen.gameObject.SetActive(false);
            else
                Destroy(oldScreen.gameObject);
        }

        public IUIScreen GetCurrentScreen()
        {
            if (_currentScreen == null) return null;
            return _currentScreen;
        }

        public bool TryGetCachedScreen<T>(out T screen) where T : MonoBehaviour, IUIScreen, ICachable
        {
            var fullName = typeof(T).FullName;
            if (_singletonScreensCache.TryGetValue(fullName, out var cachedScreen))
            {
                screen = (T)cachedScreen;
                return true;
            }

            screen = null;
            return false;
        }
        
        public bool TryGetCachedDialog<T>(out T dialog) where T : MonoBehaviour, IUIDialog, ICachable
        {
            var fullName = typeof(T).FullName;
            if (_dialogCache.TryGetValue(fullName, out var cachedDialog))
            {
                dialog = (T)cachedDialog;
                return true;
            }

            dialog = null;
            return false;
        }

        public IUIDialog GetTopDialog()
        {
            if (_dialogStack.Count == 0) return null;
            return _dialogStack[0];
        }

        public void DestroyInactiveScreen<T>()
        {
            var fullName = typeof(T).FullName;
            if (string.IsNullOrEmpty(fullName))
                throw new ArgumentException($"string.IsNullOrEmpty(fullName) for {typeof(T)}");
            if (_currentScreen.GetType().FullName == fullName)
                throw new Exception($"{Tag}->DestroyScreen: _currentScreen={_currentScreen.GetType().FullName}");
            if (!_singletonScreensCache.ContainsKey(fullName)) return;
            _singletonScreensCache.Remove(fullName);
            var destroyScreen = (MonoBehaviour)_singletonScreensCache[fullName];
            if (destroyScreen is IUIBackKeyHandler handler) RemoveBackKeyHandler(handler);
            Destroy(destroyScreen.gameObject);
        }

        public void DestroyAllInactiveScreen()
        {
            var removedList = (from e in _singletonScreensCache where e.Value != _currentScreen select e.Key).ToList();
            foreach (var e in removedList)
            {
                var destroyScreen = _singletonScreensCache[e];
                _singletonScreensCache.Remove(e);
                if (destroyScreen is IUIBackKeyHandler handler) RemoveBackKeyHandler(handler);
                Destroy(((MonoBehaviour)destroyScreen).gameObject);
            }
        }

        // -------------------------------------------------------------------------------------------------------------
        // Dialog
        // -------------------------------------------------------------------------------------------------------------
        public IAsyncOperation<T> PreloadDialog<T>() where T : MonoBehaviour, IUIDialog, ICachable
        {
            // Preload dialog into cache, returns the instance if already in cache
            var fullName = typeof(T).FullName;
            var newDialog = (T)_dialogCache.Get(fullName, null);
            if (newDialog == null)
            {
                var prefab = _prefabRepository.GetOrLoad<T>();
                newDialog = Instantiate(prefab, dialogParent);
                newDialog.gameObject.SetActive(false);
                _dialogCache.Upsert(fullName, newDialog);
            }

            return new SharedAsyncOperation<T>(newDialog);
        }

        public IAsyncOperation ShowDialogOfType(Type dialogType, DialogAction dialogAction = DialogAction.HideLastOne)
        {
            var method = typeof(UIScene).GetMethod(nameof(ShowDialog), new[] { typeof(DialogAction) })
                .MakeGenericMethod(dialogType);
            return (IAsyncOperation)method.Invoke(this, new object[] { dialogAction });
        }

        public IAsyncOperation<T> ShowDialog<T>(DialogAction dialogAction = DialogAction.HideLastOne) where T : MonoBehaviour, IUIDialog
        {
            this.LogInfo("ShowDialog: " + typeof(T).FullName + ", action=" + dialogAction);
            if (dialogAction == DialogAction.HideAll) HideAllDialog(ignoreCurrent: true);
            var oldDialog = CurrentDialog;
            var fullName = typeof(T).FullName;
            var newDialog = (T)_dialogCache.Get(fullName, null);
            if (newDialog == null)
            {
                var prefab = _prefabRepository.GetOrLoad<T>();
                newDialog = Instantiate(prefab, dialogParent);
                if (newDialog is ICachable) _dialogCache.Upsert(fullName, newDialog);
            }

            newDialog.transform.SetAsLastSibling();
            if (newDialog is ICachable) _dialogStack.Remove(newDialog);
            _dialogStack.Insert(0, newDialog);
            if (newDialog is IUIBackKeyHandler handler) AddBackKeyHandler(handler);

            if (dialogAction == DialogAction.None) oldDialog = null; // Không hide or destroy old dialog.
            if (dialogAction == DialogAction.HideLastOne) HideDialog(oldDialog);
            IAsyncOperation<T> o = new SharedAsyncOperation<T>(newDialog);
            _dialogOperations.Add(newDialog, o);
            StartCoroutine(_PresentShowDialog((MonoBehaviour)oldDialog, newDialog));
            return o;
        }

        private IEnumerator _PresentShowDialog(MonoBehaviour oldOne, MonoBehaviour newOne)
        {
            this.LogInfo("_PresentShowDialog: " + newOne.gameObject.name);
            newOne.gameObject.SetActive(true);
            if (oldOne != null)
            {
                var presenter = DialogChangedPresenter;
                if (presenter != null) yield return presenter.Present(oldOne, newOne);
                if (oldOne is IUIBackKeyHandler handler) RemoveBackKeyHandler(handler);
                if (oldOne is not ICachable) Destroy(oldOne.gameObject);
                else oldOne.gameObject.SetActive(false);
                yield break;
            }

            var showPresenter = DialogShownPresenter;
            if (showPresenter != null) yield return showPresenter.Present(newOne);
        }

        public T HideDialog<T>() where T : MonoBehaviour, IUIDialog
        {
            var fullName = typeof(T).FullName;
            var d = (T)_dialogCache.Get(fullName, null);
            if (d == null) return null;
            HideDialog(d);
            return d;
        }

        public void HideDialog(IUIDialog dialog)
        {
            if (!_dialogStack.Contains(dialog)) return;
            _dialogStack.Remove(dialog);
            StartCoroutine(_PresentHideDialog((MonoBehaviour)dialog));
        }

        public void HideCurrentDialog()
        {
            var currentDialog = CurrentDialog;
            if (currentDialog == null) return;
            HideDialog(currentDialog);
        }

        public void HideAllDialog(bool ignoreCurrent = true)
        {
            for (var i = _dialogStack.Count - 1; i > 0; i--)
                HideDialog(_dialogStack[i]);
            if (!ignoreCurrent) HideCurrentDialog();
        }

        private IEnumerator _PresentHideDialog(MonoBehaviour oldOne)
        {
            var dialog = (IUIDialog)oldOne;
            if (_dialogOperations.TryGetValue(dialog, out var o))
            {
                o.Success();
                _dialogOperations.Remove(dialog);
            }

            if (oldOne is IUIBackKeyHandler handler) RemoveBackKeyHandler(handler);
            var presenter = DialogHidePresenter;
            if (presenter != null) yield return presenter.Present(oldOne);
            oldOne.gameObject.SetActive(false);
            if (oldOne is not ICachable) Destroy(oldOne.gameObject);
        }

        public void DestroyDialog<T>() where T : MonoBehaviour, IUIDialog
        {
            var fullName = typeof(T).FullName;
            var dialog = (T)_dialogCache.Get(fullName, null);
            if (dialog == null) return;
            DestroyDialog(dialog);
        }

        public void DestroyDialog(IUIDialog dialog)
        {
            if (dialog is IUIBackKeyHandler handler) RemoveBackKeyHandler(handler);
            var fullName = dialog.GetType().FullName;
            if (string.IsNullOrEmpty(fullName)) throw new ArgumentException($"string.IsNullOrEmpty(fullName) for {dialog}");
            _dialogCache.Remove(fullName);
            _dialogStack.Remove(dialog);
            Destroy(((MonoBehaviour)dialog).gameObject);
        }

        // -------------------------------------------------------------------------------------------------------------
        // Floating Message
        // -------------------------------------------------------------------------------------------------------------
        public void ShowFloatingMessage<T>(string m) where T : MonoBehaviour, IUIFloatingMessage
        {
            var ui = _uiFloatingMessagePool.GetObject<T>();
            ui.transform.SetParent(floatingMessageParent);
            ui.Text = m;
            StartCoroutine(_PresentFloatingMessage(ui));
        }

        private IEnumerator _PresentFloatingMessage(IUIFloatingMessage m)
        {
            var castedMessage = (MonoBehaviour)m;
            castedMessage.gameObject.SetActive(true);
            castedMessage.transform.SetAsLastSibling();
            var presenter = FloatingMessagePresenter;
            if (presenter != null) yield return presenter.Present(castedMessage);
            _uiFloatingMessagePool.ReleaseObject(castedMessage);
        }

        // -------------------------------------------------------------------------------------------------------------
        // Loading Indicator
        // -------------------------------------------------------------------------------------------------------------
        public void ShowActivityIndicator<T>(string id) where T : MonoBehaviour, IUIActivityIndicator
        {
            if (_loadingIndicatorCache.ContainsKey(id)) return;
            var o = _uiActivityIndicatorPool.GetObject<T>();
            var t = (RectTransform)o.transform;
            t.SetParent(loadingParent);
            t.localScale = Vector3.one;
            t.SetAsLastSibling();
            t.SetStretchVerticalAndHorizontal();
            o.gameObject.SetActive(true);
            StartCoroutine(_PresentUILoadingIndicator(o));
            _loadingIndicatorCache.Add(id, o);
        }

        private IEnumerator _PresentUILoadingIndicator(MonoBehaviour monoBehaviour)
        {
            var presenter = LoadingIndicatorPresenter;
            if (presenter != null)
                yield return presenter.Present(monoBehaviour);
        }

        public void HideActivityIndicator(string id)
        {
            if (!_loadingIndicatorCache.ContainsKey(id)) return;
            _uiActivityIndicatorPool.ReleaseObject(_loadingIndicatorCache[id]);
            _loadingIndicatorCache.Remove(id);
        }

        // -------------------------------------------------------------------------------------------------------------
        // BackKey
        // -------------------------------------------------------------------------------------------------------------
        public void AddBackKeyHandler(IUIBackKeyHandler handler)
        {
            _backKeyHandlers.Add(handler);
        }

        public void RemoveBackKeyHandler(IUIBackKeyHandler handler)
        {
            _backKeyHandlers.Remove(handler);
        }

        private void Update() => OnBackKeyPressed();

        public bool OnBackKeyPressed()
        {
            if (!Application.isFocused || !Input.GetKeyDown(KeyCode.Escape)) return false;
            return _backKeyHandlers.Any(h => h.OnBackKeyPressed());
        }

        // -------------------------------------------------------------------------------------------------------------
        // Halo
        // -------------------------------------------------------------------------------------------------------------
        public T ShowHalo<T>(GameObject target) where T : MonoBehaviour, IUIHalo
        {
            var fullName = typeof(T).FullName;
            if (string.IsNullOrEmpty(fullName))
                throw new ArgumentException($"string.IsNullOrEmpty(fullName) for {typeof(T)}");
            var o = _haloCache.Get(fullName, null);
            if (o == null)
            {
                var prefab = _prefabRepository.GetOrLoad<T>();
                o = Instantiate(prefab, haloParent);
                _haloCache.Add(fullName, o);
            }

            (o as IUIHalo)?.Show(target);
            return (T)o;
        }

        public T ShowHaloWithButton<T>(GameObject target, Action onClickHaloCallback) where T : MonoBehaviour, IUIHalo
        {
            var fullName = typeof(T).FullName;
            if (string.IsNullOrEmpty(fullName))
                throw new ArgumentException($"string.IsNullOrEmpty(fullName) for {typeof(T)}");
            var o = _haloCache.Get(fullName, null);
            if (o == null)
            {
                var prefab = _prefabRepository.GetOrLoad<T>();
                o = Instantiate(prefab, haloParent);
                _haloCache.Add(fullName, o);
            }

            (o as IUIHalo)?.Show(target, onClickHaloCallback);
            return (T)o;
        }

        public void HideHalo(IUIHalo haloObj)
        {
            var fullName = haloObj.GetType().FullName;
            if (string.IsNullOrEmpty(fullName))
                throw new ArgumentException($"string.IsNullOrEmpty(fullName) for {haloObj.GetType()}");
            var halo = _haloCache.Get(fullName, null);
            if (halo == null) return;
            StartCoroutine(_PresentHideHalo(halo));
        }

        private IEnumerator _PresentHideHalo(MonoBehaviour monoBehaviour)
        {
            if (monoBehaviour is IUIHalo halo) yield return halo.Hide();
            monoBehaviour.gameObject.SetActive(false);
        }
    }
}