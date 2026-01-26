namespace Shared.Service.Ads.SharedLevelPlay.Banner
{
    public enum LifeCycleState
    {
        None, 
            
        Created,
            
        LoadStart,
        LoadSuccess,
        LoadFailed,
            
        DisplayStart,
        DisplaySuccess,
        DisplayFailed,
    }
}