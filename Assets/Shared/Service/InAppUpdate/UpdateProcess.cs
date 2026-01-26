using Newtonsoft.Json;

namespace Shared.Service.InAppUpdate
{
    [System.Serializable]
    public class UpdateProcess
    {
        /// <summary>
        /// [0, 100]
        /// </summary>
        [JsonProperty("progress")] public int DownloadingProgress { get; set; }

        [JsonIgnore] private UpdateState _updateState;
        [JsonIgnore] public bool UserConfirmed { get; set; }

        [JsonProperty("state")]
        public UpdateState State
        {
            get => _updateState;
            set => _updateState = value;
        }

        public void SetDownloadingProgress(int percent)
        {
            DownloadingProgress = percent;
            State = UpdateState.DownloadUpdated;
        }

        public void UpdateDownloadingProgress(int percent)
        {
            DownloadingProgress = percent;
        }
    }
}