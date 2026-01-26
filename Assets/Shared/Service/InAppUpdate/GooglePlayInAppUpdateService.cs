#if GOOGLE_PLAY && IN_APP_UPDATE && !UNITY_EDITOR
using System.Collections;
using Google.Play.AppUpdate;
using Google.Play.Common;
using Shared.Core.Async;
using Shared.Core.IoC;
using Shared.Core.Validator;
using Shared.Entity.InAppUpdate;
using Shared.Repository.InAppUpdate;
using Shared.Service.InAppUpdate.GooglePlay;
using Shared.Service.InAppUpdate.Validation;
using Shared.Service.SharedCoroutine;
using Shared.Utils;
using UnityEngine;
using Zenject;

namespace Shared.Service.InAppUpdate
{
    [Service]
    public class GooglePlayInAppUpdateService : IInAppUpdateService, ISharedUtility
    {
        private readonly WaitForSecondsRealtime _waitSeconds = new(1f);
        
        [Inject] private InAppUpdateConfigRepository _configRepository;
        private readonly IValidator _validators = ValidatorChain.CreateChainFromType<IInAppUpdateValidator>();

        private AppUpdateManager _appUpdateManager;
        private IAsyncOperation<UpdateProcess> _updateOperation;
        
        public IAsyncOperation<UpdateProcess> HandleNewVersionIfExisted()
        {
            if (_updateOperation != null)
                return _updateOperation;
            var c = _configRepository.Get();
            if (!c.Unlocked || !_validators.Validate())
            {
                this.LogInfo(SharedLogTag.InAppUpdate, nameof(c.Unlocked), c.Unlocked);
                return new SharedAsyncOperation<UpdateProcess>(new UpdateProcess { State = UpdateState.UpdateFailed}).Fail("!c.Unlocked");   
            }

            _updateOperation = new SharedAsyncOperation<UpdateProcess>(new UpdateProcess());
            _appUpdateManager = new AppUpdateManager();
            this.StartSharedCoroutine(_JumpInCheckForUpdateState(c));
            return _updateOperation;
        }

        public bool ConfirmUpdateByUser()
        {
            if (_updateOperation == null) return false;
            if (_updateOperation.Data == null) return false;
            _updateOperation.Data.UserConfirmed = true;
            return true;
        }

        private IEnumerator _JumpInCheckForUpdateState(InAppUpdateConfig config)
        {
            this.LogInfo(SharedLogTag.InAppUpdate, "jumpIn", "CheckForUpdateState");
            _updateOperation.Data.State = UpdateState.CheckForUpdate;
            var getAppUpdateInfoOperation = _appUpdateManager.GetAppUpdateInfo();
            yield return getAppUpdateInfoOperation;
            this.LogInfo(SharedLogTag.InAppUpdate, nameof(getAppUpdateInfoOperation), getAppUpdateInfoOperation.ToDict());
            
            if (!getAppUpdateInfoOperation.IsSuccessful)
            {    
                _JumpInFailedState("CheckForUpdateState", "!getAppUpdateInfoOperation.IsSuccessful");
                yield break;
            }
            
            var appUpdateInfo = getAppUpdateInfoOperation.GetResult();
            this.LogInfo(SharedLogTag.InAppUpdate, nameof(appUpdateInfo), appUpdateInfo.ToString());
            
            if (appUpdateInfo.UpdateAvailability == UpdateAvailability.UpdateAvailable)
            {
                var currentVersionCode = AndroidUtils.GetVersionCode();
                var isHardUpdate = config.MinVersionCode > currentVersionCode;
                this.LogInfo(SharedLogTag.InAppUpdate, nameof(isHardUpdate), isHardUpdate, nameof(config.MinVersionCode), config.MinVersionCode, nameof(currentVersionCode), currentVersionCode);

                if (isHardUpdate)
                    yield return _JumpInImmediateUpdateState(appUpdateInfo);
                else
                    yield return _JumpInFlexibleUpdateState(appUpdateInfo);
            }
            else if (appUpdateInfo.UpdateAvailability == UpdateAvailability.DeveloperTriggeredUpdateInProgress)
            {
                this.LogInfo(SharedLogTag.InAppUpdate, "DeveloperTriggeredUpdateInProgress");
                yield return _HandleResumeUpdate(appUpdateInfo);
            }
            else
            {
                switch (appUpdateInfo.AppUpdateStatus)
                {
                    case AppUpdateStatus.Downloading:
                        yield return _HandleResumeUpdate(appUpdateInfo);
                        yield break;

                    case AppUpdateStatus.Downloaded:
                        yield return _JumpInCompleteUpdateState();
                        yield break;

                    case AppUpdateStatus.Installing:
                    case AppUpdateStatus.Installed:
                    case AppUpdateStatus.Unknown or AppUpdateStatus.Pending or AppUpdateStatus.Failed or AppUpdateStatus.Canceled:
                    default:
                        this.LogInfo(SharedLogTag.InAppUpdate, nameof(appUpdateInfo.AppUpdateStatus), appUpdateInfo.AppUpdateStatus.ToString(), nameof(getAppUpdateInfoOperation), getAppUpdateInfoOperation.ToDict());
                        _JumpInFailedState("CheckForUpdateState", appUpdateInfo.AppUpdateStatus.ToString());
                        yield break;
                }
            }
        }

        private IEnumerator _HandleResumeUpdate(AppUpdateInfo appUpdateInfo)
        {
            this.LogInfo(SharedLogTag.InAppUpdate, "HandleResumeUpdate", "status", ((int)appUpdateInfo.AppUpdateStatus).ToString(), "name", appUpdateInfo.AppUpdateStatus.ToString());
    
            switch (appUpdateInfo.AppUpdateStatus)
            {
                case AppUpdateStatus.Unknown:
                case AppUpdateStatus.Pending:
                    _updateOperation.Data.State = UpdateState.DownloadStarted;
                    yield return _TrackFlexibleProgress();
                    break;

                case AppUpdateStatus.Downloading:
                    _updateOperation.Data.State = UpdateState.DownloadStarted;
                    yield return _TrackFlexibleProgress();
                    break;

                case AppUpdateStatus.Downloaded:
                    _updateOperation.Data.State = UpdateState.DownloadCompleted;
                    yield return _JumpInCompleteUpdateState();
                    break;

                case AppUpdateStatus.Installing:
                    _updateOperation.Data.State = UpdateState.UpdateStarted;
                    yield return new WaitForSeconds(2f);
                    break;

                case AppUpdateStatus.Installed:
                    yield break;

                case AppUpdateStatus.Failed:
                case AppUpdateStatus.Canceled:
                    _JumpInFailedState("HandleResumeUpdate", appUpdateInfo.AppUpdateStatus.ToString());
                    break;

                default:
                    this.LogWarning(SharedLogTag.InAppUpdate, "Unexpected resume status", appUpdateInfo.AppUpdateStatus.ToString());
                    _JumpInFailedState("HandleResumeUpdate", "Unexpected status: " + appUpdateInfo.AppUpdateStatus);
                    break;
            }
        }

        private IEnumerator _TrackFlexibleProgress()
        {
            this.LogInfo(SharedLogTag.InAppUpdate, "TrackFlexibleProgress");
            int fakePercent = 0;
            float lastTick = Time.realtimeSinceStartup;

            while (true)
            {
                if (_updateOperation == null)
                    yield break;

                var op = _appUpdateManager.GetAppUpdateInfo();
                yield return op;
                this.LogInfo(SharedLogTag.InAppUpdate, "TrackFlexibleProgress", nameof(op), op.ToDict());

                if (!op.IsSuccessful)
                {
                    this.LogError(SharedLogTag.InAppUpdate, nameof(_TrackFlexibleProgress), "GetAppUpdateInfo failed", op.ToDict());
                    yield return _waitSeconds;
                    _waitSeconds.Reset();
                    continue;
                }

                var appUpdateInfo = op.GetResult();
                this.LogInfo(SharedLogTag.InAppUpdate, "TrackFlexibleProgress", "AppUpdateStatus", ((int)appUpdateInfo.AppUpdateStatus).ToString(), "Name", appUpdateInfo.AppUpdateStatus.ToString());

                switch (appUpdateInfo.AppUpdateStatus)
                {
                    case AppUpdateStatus.Unknown:
                        this.LogInfo(SharedLogTag.InAppUpdate, "Status Unknown, continue polling");
                        break;

                    case AppUpdateStatus.Pending:
                        this.LogInfo(SharedLogTag.InAppUpdate, "Status Pending, waiting for download to start");
                        break;

                    case AppUpdateStatus.Downloading:
                        int realPercent = 0;

                        // Calculate the real download progress
                        if (appUpdateInfo.TotalBytesToDownload > 0)
                        {
                            realPercent = (int)((float)appUpdateInfo.BytesDownloaded / appUpdateInfo.TotalBytesToDownload * 100f);
                        }

                        // Simulate a fake download progress for demonstration purposes
                        float now = Time.realtimeSinceStartup;
                        if (now - lastTick >= 1f)
                        {
                            lastTick = now;
                            int step = fakePercent < 70
                                ? Random.Range(5, 10)   
                                : (fakePercent < 90
                                    ? Random.Range(2, 5)  
                                    : Random.Range(1, 2)  
                                );
                            fakePercent = Mathf.Min(fakePercent + step, 99);
                        }

                        int percent = Mathf.Clamp(Mathf.Max(realPercent, fakePercent), 0, 99);
                        _updateOperation.Data.SetDownloadingProgress(percent);

                        this.LogInfo(SharedLogTag.InAppUpdate, "Downloading", "Progress", percent + "%", "Downloaded", appUpdateInfo.BytesDownloaded, "Total", appUpdateInfo.TotalBytesToDownload);
                        break;

                    case AppUpdateStatus.Downloaded:
                        _updateOperation.Data.State = UpdateState.DownloadCompleted;
                        this.LogInfo(SharedLogTag.InAppUpdate, "Download completed, requesting user action");
                        yield return _JumpInUserActionRequiredState();
                        yield break;

                    case AppUpdateStatus.Installing:
                        if (_updateOperation.Data.State != UpdateState.UpdateStarted)
                            _updateOperation.Data.State = UpdateState.UpdateStarted;
                        this.LogInfo(SharedLogTag.InAppUpdate, "Installing update");
                        yield break;

                    case AppUpdateStatus.Installed:
                        this.LogInfo(SharedLogTag.InAppUpdate, "Update installed, app will restart");
                        yield break;

                    case AppUpdateStatus.Failed:
                    case AppUpdateStatus.Canceled:
                        this.LogError(SharedLogTag.InAppUpdate, "Update failed or canceled", "Status", appUpdateInfo.AppUpdateStatus.ToString());
                        _JumpInFailedState(nameof(_TrackFlexibleProgress), appUpdateInfo.AppUpdateStatus.ToString());
                        yield break;

                    default:
                        this.LogWarning(SharedLogTag.InAppUpdate, "Unexpected AppUpdateStatus", appUpdateInfo.AppUpdateStatus.ToString());
                        break;
                }

                yield return _waitSeconds;
                _waitSeconds.Reset();
            }              
        }

        private IEnumerator _JumpInFlexibleUpdateState(AppUpdateInfo appUpdateInfo)
        {
            _updateOperation.Data.State = UpdateState.DownloadStarted;

            this.LogInfo(SharedLogTag.InAppUpdate, "jumpIn", "FlexibleUpdateState");
            var startUpdateRequest = _appUpdateManager.StartUpdate(appUpdateInfo, AppUpdateOptions.FlexibleAppUpdateOptions());

            while (!startUpdateRequest.IsDone)
            {
                switch (startUpdateRequest.Status)
                {
                    case AppUpdateStatus.Downloading:
                        ulong totalBytes = startUpdateRequest.TotalBytesToDownload;
                        ulong downloadedBytes = startUpdateRequest.BytesDownloaded;
                        int progressPercent = 0;
                        if (totalBytes > 0)
                        {
                            progressPercent = (int)((float)downloadedBytes / totalBytes * 100);
                        }

                        _updateOperation.Data.SetDownloadingProgress(progressPercent);
                        if (_updateOperation.Data.State != UpdateState.DownloadUpdated)
                        {
                            _updateOperation.Data.State = UpdateState.DownloadUpdated;
                        }

                        this.LogInfo(SharedLogTag.InAppUpdate, "Download progress", "percent", progressPercent);
                        break;

                    case AppUpdateStatus.Downloaded:
                        _updateOperation.Data.State = UpdateState.DownloadCompleted;
                        yield return _JumpInUserActionRequiredState();
                        yield break;
                }

                yield return _waitSeconds;
                _waitSeconds.Reset();
            }

            this.LogInfo(SharedLogTag.InAppUpdate, "state", _updateOperation.Data.State.ToString(), nameof(startUpdateRequest.Status), startUpdateRequest.Status.ToString());
            if (startUpdateRequest.Status == AppUpdateStatus.Failed)
            {
                _JumpInFailedState(_updateOperation.Data.State.ToString(), "updateRequest.Status == AppUpdateStatus.Failed");
            }
            else
            {
                yield return _JumpInUserActionRequiredState();
            }
        }
        
        // private IEnumerator _JumpInFlexibleBackgroundUpdate(AppUpdateInfo appUpdateInfo)
        // {
        //     UpdateOperation.Data.State = UpdateState.DownloadStarted;
        //
        //     var allowLoop = true;
        //     var loopCount = 0;
        //     while (allowLoop)
        //     {
        //         var appUpdateInfoOperation = AppUpdateManager.GetAppUpdateInfo();
        //         // wait until the asynchronous operation completes.
        //         yield return appUpdateInfoOperation;
        //
        //         if (!appUpdateInfoOperation.IsSuccessful)
        //         {
        //             context.UpdateOperation.Data.State = UpdateState.UpdateFailed;
        //             context.UpdateOperation.Fail(string.Empty);
        //             onStateComplete(null); // Exit state machine
        //             yield break;
        //         }
        //         this.LogInfo(SharedLogTag.InAppUpdate, nameof(appUpdateInfoOperation), appUpdateInfoOperation.ToDict());
        //
        //         var appUpdateInfo = appUpdateInfoOperation.GetResult();
        //         switch (appUpdateInfo.AppUpdateStatus)
        //         {
        //             case AppUpdateStatus.Downloading:
        //             {
        //                 loopCount++;
        //                 if (loopCount > 99) loopCount = 99;
        //                 context.UpdateOperation.Data.SetDownloadingProgress(loopCount);
        //                 break;
        //             }
        //             case AppUpdateStatus.Downloaded:
        //             {
        //                 allowLoop = false;
        //                 context.UpdateOperation.Data.State = UpdateState.DownloadCompleted;
        //                 onStateComplete(new UserActionRequireState());
        //                 yield break;
        //             }
        //             case AppUpdateStatus.Unknown:
        //             case AppUpdateStatus.Pending:
        //             case AppUpdateStatus.Installing:
        //             case AppUpdateStatus.Installed:
        //             case AppUpdateStatus.Failed:
        //             case AppUpdateStatus.Canceled:
        //             default:
        //                 allowLoop = false;
        //                 context.UpdateOperation.Data.State = UpdateState.UpdateFailed;
        //                 context.UpdateOperation.Fail(string.Empty);
        //                 onStateComplete(null); // Exit state machine
        //                 break;
        //         }
        //         yield return _wait5Seconds;
        //     }
        // }
        
        private IEnumerator _JumpInUserActionRequiredState()
        {
            this.LogInfo(SharedLogTag.InAppUpdate, "jumpIn", "UserActionRequiredState");
            _updateOperation.Data.State = UpdateState.UserActionRequired;
            while (_updateOperation is { Data: { UserConfirmed: false } }) yield return null;
            if (_updateOperation?.Data?.UserConfirmed ?? false)
                yield return _JumpInCompleteUpdateState();
        }
        
        private IEnumerator _JumpInCompleteUpdateState()
        {
            this.LogInfo(SharedLogTag.InAppUpdate, "jumpIn", "CompleteUpdateState");
            _updateOperation.Data.State = UpdateState.UpdateStarted;
            var result = _appUpdateManager.CompleteUpdate();
            yield return result;
            // If the update completes successfully, then the app restarts and this line
            // is never reached. If this line is reached, then handle the failure (e.g. by
            // logging result.Error or by displaying a message to the user).
            this.LogError(SharedLogTag.InAppUpdate, "f", nameof(_JumpInCompleteUpdateState), nameof(result), result.ToDict());
            _JumpInFailedState(_updateOperation.Data.State.ToString(), result.Error.ToString());
            // context.UpdateOperation.Data.State = UpdateState.UpdateFailed;
            // context.UpdateOperation.Fail(string.Empty);
            // onStateComplete(null); // Exit state machine
        }
        
        public IEnumerator _JumpInImmediateUpdateState(AppUpdateInfo _appUpdateInfo)
        {
            this.LogInfo(SharedLogTag.InAppUpdate, "jumpIn", "ImmediateUpdateState");
            _updateOperation.Data.State = UpdateState.UpdateStarted;
            // Creates an AppUpdateRequest that can be used to monitor the requested in-app update flow.
            var updateRequest = _appUpdateManager.StartUpdate(_appUpdateInfo, AppUpdateOptions.ImmediateAppUpdateOptions());
            yield return updateRequest;
            // If the update completes successfully, then the app restarts and this line
            // is never reached. If this line is reached, then handle the failure (for
            // example, by logging result.Error or by displaying a message to the user).
            this.LogError(SharedLogTag.InAppUpdate, "f", nameof(_JumpInImmediateUpdateState), nameof(updateRequest), updateRequest.ToDict());
            _JumpInFailedState(_updateOperation.Data.State.ToString(), updateRequest.Status.ToString());
            // context.UpdateOperation.Data.State = UpdateState.UpdateFailed;
            // context.UpdateOperation.Fail($"{nameof(ImmediateUpdateState)}");
            // onStateComplete(null); // Exit state machine
        }

        private void _JumpInFailedState(string fromState = "", string reason = "")
        {
            this.LogInfo(SharedLogTag.InAppUpdate, "jumpIn", "FailedState");
            _updateOperation.Data.State = UpdateState.UpdateFailed;
            _updateOperation.Fail($"{fromState} {reason}");
            _updateOperation = null;
        }
    }
}
#endif