#if IN_APP_UPDATE && UNITY_EDITOR
using System.Collections;
using Shared.Core.Async;
using Shared.Core.IoC;
using Shared.Service.SharedCoroutine;
using UnityEngine;

namespace Shared.Service.InAppUpdate
{
    [Service]
    public class FakeInAppUpdateService : IInAppUpdateService, ISharedUtility
    {
        public IAsyncOperation<UpdateProcess> UpdateOperation { get; private set; }
        private bool _userConfirm = false;
        

        IAsyncOperation<UpdateProcess> IInAppUpdateService.HandleNewVersionIfExisted()
        {
            if (UpdateOperation != null) return UpdateOperation;
            UpdateOperation = new SharedAsyncOperation<UpdateProcess>(new UpdateProcess());

            this.StartSharedCoroutine(_Simulate());
            
            return UpdateOperation;
        }

        private IEnumerator _Simulate()
        {
            _userConfirm = false;
            
            UpdateOperation.Data.State = UpdateState.CheckForUpdate;
            yield return new WaitForSecondsRealtime(3f);
            
            UpdateOperation.Data.State = UpdateState.DownloadStarted;
            yield return new WaitForSecondsRealtime(1f);

            for (var i = 1; i <= 100; i++)
            {
                UpdateOperation.Data.SetDownloadingProgress(i);
                yield return new WaitForSecondsRealtime(.2f);
            }
            
            UpdateOperation.Data.State = UpdateState.DownloadCompleted;
            yield return new WaitForSecondsRealtime(.2f);
            UpdateOperation.Data.State = UpdateState.UserActionRequired;
            
            yield return new WaitUntil(() => _userConfirm);
            UpdateOperation.Data.State = UpdateState.UpdateStarted;
            
            yield return new WaitForSecondsRealtime(3f);
            UpdateOperation.Data.State = UpdateState.UpdateFailed;
        }

        public bool ConfirmUpdateByUser()
        {
            _userConfirm = true;
            return true;
        }
    }
}
#endif