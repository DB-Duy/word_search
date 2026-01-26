using System.Collections.Generic;
using System.Linq;
using Shared.Core.Async;
using Shared.Core.IoC;
using Shared.Repository.GameInterrupt;
using Shared.Service.Crash;
using Shared.Service.Tracking;
using Shared.Tracking.Models.Game;
using Shared.Tracking.Property;
using Shared.Utilities;
using Shared.Utils;
using Zenject;

namespace Shared.Service.GameInterrupt
{
    [Service]
    public class GameInterruptService : IGameInterruptService, ISharedUtility, ISharedLogTag
    {
        public string LogTag => SharedLogTag.TrackingNInterrupt;
        
        [Inject] private GameInterruptRepository _repository;
        [Inject] private ICrashService _crashService;
        [Inject] private ITrackingService _trackingService;
        
        public GameInterruptService()
        {
            SharedCoreEvents.Session.OnNewSessionCreatedEvent += UpdateSessionId;
        }
        
        public bool IsInitialized { get; private set; } = false;
        public IAsyncOperation Initialize()
        {
            if (IsInitialized) return new SharedAsyncOperation().Success();
            IsInitialized = true;
            this.LogInfo(nameof(IsInitialized), IsInitialized);
            TrackGameInterrupt();
            return new SharedAsyncOperation().Success();
        }

        public void PrepareGameInterrupt(Dictionary<string, object> data)
        {
            this.LogInfo(nameof(data), data);
            _repository.Save(data);
        }

        public void TrackGameInterrupt()
        {
            var crashReason = _crashService.GetCrashReason();
            var data = _repository.Get() ?? new Dictionary<string, object>();
            var p = new GameInterruptedParams(crashReason, data);
            _trackingService.Track(p);
            this.LogInfo(nameof(crashReason), crashReason, nameof(data), data);
        }

        public Dictionary<string, object> GetGameInterruptedData() => _repository.Get();
        
        public void UpdateSessionId(long sessionId)
        {
            var cachedData = _repository.Get();
            cachedData[PropertyConst.SESSION_ID] = sessionId;
            this.LogInfo(nameof(cachedData), cachedData);
            _repository.Save(cachedData);
        }

        public void UpdateWith(GameStartParams e, params string[] cloneParams)
        {
            var cachedData = _repository.Get();
            var p = e.ToConvertableEvent();
            foreach (var pName in cloneParams.Where(pName => p.ContainsKey(pName)))
            {
                cachedData.Upsert(pName, p[pName]);
            }
            this.LogInfo(nameof(cachedData), cachedData);
            _repository.Save(cachedData);
        }
    }
}