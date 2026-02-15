using System;
using System.Collections;
using System.Collections.Generic;
using Shared.Core.Async;
using Shared.Core.View.Common;
using Shared.Core.View.Dialog;
using Shared.Core.View.Scene;
using Shared.Service.SharedCoroutine;
using Shared.Utils;
using UnityEngine;

namespace Shared.Core.View.DialogQueue
{
    public class DialogQueueOperation
    {
        public IQueueableDialog Dialog;
        public Func<bool> Validator;
        public Action OnShowCallback;
    }

    public class DialogQueue : ISharedUtility
    {
        public static readonly string Tag = "DialogQueue";
        public Action OnQueueCompleted;
        public Queue<DialogQueueOperation> PopUpsQueue { get; private set; }
        protected IQueueableDialog _currentDialog;
        public bool IsShowing { get; private set; } = false;
        protected Coroutine _currentDialogCoroutine;
        private bool _clearOnComplete = false;

        public DialogQueue()
        {
            PopUpsQueue = new();
        }

        public virtual void InterruptDialogQueue()
        {
            Clear();
            if (_currentDialog != null)
            {
                _currentDialog.OnDialogHide += _InterruptLastMenuCallback;
            }
            else
            {
                IsShowing = false;
            }
        }

        public virtual bool ContainsDialog<T>(T dialog) where T : IQueueableDialog
        {
            foreach (var operation in PopUpsQueue)
            {
                if (operation.Dialog.Equals(dialog)) return true;
            }

            return false;
        }

        public virtual bool ContainsDialog(IQueueableDialog dialog)
        {
            foreach (var operation in PopUpsQueue)
            {
                if (operation.Dialog.Equals(dialog)) return true;
            }

            return false;
        }

        public virtual void AppendDialog<T>(Func<bool> validator, Action onShowCallback = null) where T : MonoBehaviour, IQueueableDialog, IUIDialog, ICachable
        {
            var op = UIScene.Instance.PreloadDialog<T>();
            var dialog = op.Data;
            var queueable = dialog as IQueueableDialog;
            if (dialog == null || queueable == null)
            {
                this.LogError($"DialogQueue->AppendDialog: Dialog of type {typeof(T).Name} not found in UIScene.");
                return;
            }

            queueable.CurrentDialogQueue = this;
            PopUpsQueue.Enqueue(new DialogQueueOperation
            {
                Dialog = queueable,
                Validator = validator,
                OnShowCallback = onShowCallback
            });
        }

        public virtual void Clear()
        {
            foreach (var operation in PopUpsQueue)
            {
                operation.Dialog.CurrentDialogQueue = null;
            }

            PopUpsQueue.Clear();
            OnQueueCompleted = null;

            SharedLogger.Log($"DialogQueue->Clear");
        }

        protected virtual void _InterruptLastMenuCallback()
        {
            IsShowing = false;
            _currentDialog.OnDialogHide -= _InterruptLastMenuCallback;
        }

        public bool ClearOnComplete
        {
            get => _clearOnComplete;
            set => _clearOnComplete = value;
        }

        public virtual void ShowDialogQueue()
        {
            if (IsShowing) return;
            if (PopUpsQueue.Count <= 0)
            {
                _CompleteQueue();
                return;
            }


            this.LogInfo("Showing Dialog Queue");
            _currentDialogCoroutine = this.StartSharedCoroutine(ShowDialogCoroutine());
        }

        protected virtual IEnumerator ShowDialogCoroutine()
        {
            while (!_CanShowNextDialog()) yield return null;

            IsShowing = true;

            if (PopUpsQueue.Count == 0)
            {
                _CompleteQueue();
                yield break;
            }

            var operation = PopUpsQueue.Peek();
            while (!operation.Validator.Invoke())
            {
                operation.Dialog.CurrentDialogQueue = null;
                PopUpsQueue.Dequeue();
                if (PopUpsQueue.Count == 0)
                {
                    _CompleteQueue();
                    yield break;
                }

                operation = PopUpsQueue.Peek();
            }

            _currentDialog = operation.Dialog;
            this.LogInfo("Showing Dialog: " + _currentDialog.GetType().Name);
            UIScene.Instance.ShowDialogOfType(_currentDialog.GetType());
            operation.OnShowCallback?.Invoke();
            operation.Dialog.OnDialogHide += _OnHideDialog;
            yield break;
        }

        protected virtual void _CompleteQueue()
        {
            OnQueueCompleted?.Invoke();
            IsShowing = false;
            OnQueueCompleted = null;
            if (ClearOnComplete)
            {
                Clear();
            }
            SharedLogger.Log($"{Tag}->_CompleteQueue");
        }

        protected virtual bool _CanShowNextDialog()
        {
            return true;
        }

        protected virtual void _ShowNextDialog()
        {
            SharedLogger.LogJson($"DialogQueue->_ShowNextDialog", nameof(PopUpsQueue.Count), PopUpsQueue.Count);
            if (PopUpsQueue.Count <= 0)
            {
                _CompleteQueue();
                return;
            }

            _currentDialogCoroutine = this.StartSharedCoroutine(ShowDialogCoroutine());
        }

        protected virtual void _OnHideDialog()
        {
            if (PopUpsQueue.Count > 0)
            {
                PopUpsQueue.Dequeue();
            }

            _currentDialog.OnDialogHide -= _OnHideDialog;
            _currentDialog.CurrentDialogQueue = null;
            _ShowNextDialog();
        }

        public virtual void InsertDialogAfter<T>(IQueueableDialog dialogBefore, Func<bool> validator, Action onShowCallback = null) where T : MonoBehaviour, IQueueableDialog, IUIDialog, ICachable
        {
            var op = UIScene.Instance.PreloadDialog<T>();
            var dialog = op.Data;
            var queueable = dialog as IQueueableDialog;
            InsertDialogAfter(dialogBefore, queueable, validator, onShowCallback);
        }

        public virtual void InsertDialogAfter(IQueueableDialog dialog, IQueueableDialog nextDialog, Func<bool> validator, Action onShowCallback = null)
        {
            if (dialog.CurrentDialogQueue == null || !dialog.CurrentDialogQueue.ContainsDialog(dialog) || dialog.CurrentDialogQueue.ContainsDialog(nextDialog))
                return;

            var queueList = new List<DialogQueueOperation>(dialog.CurrentDialogQueue.PopUpsQueue);
            var existingDialogIndex = queueList.FindIndex(item => item.Dialog.Equals(dialog));

            if (existingDialogIndex == -1) return;

            queueList.Insert(existingDialogIndex + 1, new DialogQueueOperation
            {
                Dialog = nextDialog,
                Validator = validator,
                OnShowCallback = onShowCallback
            });

            dialog.CurrentDialogQueue.PopUpsQueue = new Queue<DialogQueueOperation>(queueList);
        }
    }
}