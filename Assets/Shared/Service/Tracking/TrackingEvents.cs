using System;
using Shared.Tracking.Models.Game;

namespace Shared.Service.Tracking
{
    public static class TrackingEvents
    {
        public static Action<GameScreenParams> OnGameScreenTrackedEvent = delegate {  };
    }
}