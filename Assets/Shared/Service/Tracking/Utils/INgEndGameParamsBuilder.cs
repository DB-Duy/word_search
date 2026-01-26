namespace Shared.Tracking.Utils
{
    public interface INgEndGameParamsBuilder : ISpecificEndgameParamsBuilder
    {
        INgEndGameParamsBuilder SetResult(string result);
        INgEndGameParamsBuilder SetLiveRemain(int liveRemain);
        INgEndGameParamsBuilder SetCountX(int countX);
        INgEndGameParamsBuilder SetContinueCount(int continueCount);
    }
}