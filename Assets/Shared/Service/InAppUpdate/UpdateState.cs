namespace Shared.Service.InAppUpdate
{
    public enum UpdateState
    {
        None,
        
        CheckForUpdate,
        
        DownloadStarted,
        DownloadUpdated,
        DownloadCompleted,
        
        UserActionRequired,
        
        UpdateStarted,
        UpdateFailed
    }
}