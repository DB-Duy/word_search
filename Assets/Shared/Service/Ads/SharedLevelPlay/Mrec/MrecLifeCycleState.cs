namespace Shared.Service.Ads.SharedLevelPlay.Mrec
{
    public enum MrecLifeCycleState
    {
        None, 
            
        Created,
            
        LoadStart,
        LoadSuccess,
        LoadFailed,
            
        DisplayStart,
        DisplaySuccess,
        DisplayFailed,
        Hidden
    }
}