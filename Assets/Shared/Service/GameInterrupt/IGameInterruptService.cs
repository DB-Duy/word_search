using System.Collections.Generic;
using Shared.Core.Handler.Corou.Initializable;
using Shared.Tracking.Models.Game;

namespace Shared.Service.GameInterrupt
{
    public interface IGameInterruptService : IInitializable
    {
        void PrepareGameInterrupt(Dictionary<string, object> data);

        void TrackGameInterrupt();

        Dictionary<string, object> GetGameInterruptedData();

        void UpdateSessionId(long sessionId);

        void UpdateWith(GameStartParams e, params string[] cloneParams);
    }
}