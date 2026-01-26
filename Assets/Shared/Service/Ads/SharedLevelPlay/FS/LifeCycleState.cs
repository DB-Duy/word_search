namespace Shared.Service.Ads.SharedLevelPlay.FS
{
    public enum LifeCycleState
    {
        Created,
        
        LoadStart,
        LoadSuccess,
        LoadFail,
        
        ShowStart,
        ShowSuccess,
        ShowFail,
        
        Destroyed
    }
}