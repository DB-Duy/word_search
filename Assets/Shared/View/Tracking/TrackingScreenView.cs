using Shared.Service.Tracking;
using Shared.Service.Tracking.Common;
using Shared.Tracking.Models.Game;
using Shared.Utils;
using UnityEngine;

namespace Shared.View.Tracking
{
    [DisallowMultipleComponent]
    public class TrackingScreenView : MonoBehaviour, ISharedUtility
    {
        [SerializeField] private string screenName;

        private void OnEnable()
        {
            this.LogInfo("eventName", "game_screen", nameof(screenName), screenName);
            this.Track(new GameScreenParams(new TrackingScreen(screenName)));
        }
    }
}