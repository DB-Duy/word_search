namespace Shared.Service.Loading
{
    [System.Serializable]
    public class LoadOperation
    {
        public LoadState LoadState { get; set; } = LoadState.None;
        public int LoadingPercent { get; set; } = 0;
        public float LoadingPercentFloat => LoadingPercent / 100f;
    }
}