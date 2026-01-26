namespace Shared.Service.Ads.SharedLevelPlay.RV
{
    public enum LifeCycleState
    {
        Initialized,
        Created,
        
        LoadStarted,
        LoadSucceeded,
        LoadFailed,
        
        ShowStarted,
        ShowSucceeded,
        ShowFailed,
        
        Destroyed,
    }
}