namespace Shared.View.InPlayAds
{
    public interface IInPlayAd
    {
        bool IsReady { get; }
        string ForPlacementName { get; }
        void ResetReadyState();
    }
}