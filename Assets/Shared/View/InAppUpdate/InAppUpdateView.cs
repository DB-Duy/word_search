using System.Collections;
using Shared.Core.IoC;
using Shared.Service.InAppUpdate;
using Shared.Utils;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Shared.View.InAppUpdate
{
    public class InAppUpdateView : IoCMonoBehavior
    {   
        [System.Serializable]
        public class DownloadGroup
        {
            [SerializeField] internal GameObject rootObject;
            [SerializeField] internal TextMeshProUGUI downloadText;
        }

        [System.Serializable]
        public class UpdateGroup
        {
            [SerializeField] internal GameObject rootObject;
            [SerializeField] internal Button updateButton;
        }

        [SerializeField] private bool triggerOnEnable = true;
        [SerializeField] private GameObject rootObject;
        [SerializeField] private DownloadGroup downloadGroup;
        [SerializeField] private UpdateGroup updateGroup;
        
        
#if UNITY_ANDROID
        [Inject] private IInAppUpdateService _service;
        private UpdateState _currentState = UpdateState.None;

        private void Start()
        {
            updateGroup.updateButton.onClick.AddListener(() => _service.ConfirmUpdateByUser());
        }

        private void OnEnable()
        {
            if (!triggerOnEnable)
                return;

            rootObject.SetActive(false);
            updateGroup.updateButton.enabled = false;
            _currentState = UpdateState.None;
            StartCoroutine(_Run());
        }
        
        private IEnumerator _Run()
        {
            this.LogInfo(SharedLogTag.InAppUpdate, "f", "Step1");
            var o = _service.HandleNewVersionIfExisted();
            if (o == null)
            {
                this.LogError(SharedLogTag.InAppUpdate, "Update operation is null");
                yield break;
            }

            while (o.Data.State is UpdateState.None or UpdateState.CheckForUpdate)
                yield return null;
            
            this.LogInfo(SharedLogTag.InAppUpdate, "f", "Step2   " + o.Data.State.ToString());
            while (o.Data.State == UpdateState.DownloadStarted)
            {
                if (_currentState != UpdateState.DownloadStarted)
                {
                    _currentState = UpdateState.DownloadStarted;
                    rootObject.SetActive(true);
                    downloadGroup.rootObject.SetActive(true);
                    updateGroup.rootObject.SetActive(false);
                    downloadGroup.downloadText.text = "0%";
                    updateGroup.updateButton.enabled = false;
                }
                yield return null;
            }

            while (o.Data.State == UpdateState.UpdateStarted)
            {
                if (_currentState != UpdateState.UpdateStarted)
                {
                    _currentState = UpdateState.UpdateStarted;
                    rootObject.SetActive(true);
                    downloadGroup.rootObject.SetActive(true);
                    updateGroup.rootObject.SetActive(false);
                    downloadGroup.downloadText.text = $"{o.Data.DownloadingProgress}%";
                    updateGroup.updateButton.enabled = false;
                }
                
                yield return null;
            }

            while (o.Data.State == UpdateState.DownloadUpdated)
            {
                if (_currentState != UpdateState.DownloadUpdated)
                {
                    _currentState = UpdateState.DownloadUpdated;
                    rootObject.SetActive(true);
                    downloadGroup.rootObject.SetActive(true);
                    updateGroup.rootObject.SetActive(false);
                    updateGroup.updateButton.enabled = false;
                }
                downloadGroup.downloadText.text = $"{o.Data.DownloadingProgress}%";
                yield return null;
            }

            while (o.Data.State == UpdateState.DownloadCompleted)
            {
                if (_currentState != o.Data.State)
                {
                    _currentState = o.Data.State;
                    rootObject.SetActive(true);
                    downloadGroup.rootObject.SetActive(true);
                    updateGroup.rootObject.SetActive(false);
                    updateGroup.updateButton.enabled = false;
                }
                yield return null;
            }
            
            while (o.Data.State == UpdateState.UserActionRequired)
            {
                if (_currentState != o.Data.State)
                {
                    _currentState = o.Data.State;
                    rootObject.SetActive(true);
                    downloadGroup.rootObject.SetActive(false);
                    updateGroup.rootObject.SetActive(true);
                    updateGroup.updateButton.enabled = true;
                }
                yield return null;
            }
            
            rootObject.SetActive(false);
            yield return null;
        }
#endif
    }
}