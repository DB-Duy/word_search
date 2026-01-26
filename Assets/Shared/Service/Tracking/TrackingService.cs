using System.Diagnostics;
using Shared.Core.Handler;
using Shared.Core.IoC;
using Shared.Service.Tracking.Common;
using Shared.Service.Tracking.EventModify;
using Shared.Service.Tracking.Handler;
using Shared.Tracking.Models.Game;
using Shared.Tracking.Templates;
using Shared.Utils;

namespace Shared.Service.Tracking
{
    [Service(lazy: false)]
    public class TrackingService : ITrackingService, ISharedUtility
    {
        private IHandler<string, object> _userPropertyHandler;
        private IHandler<string, object> UserPropertyHandler => _userPropertyHandler ??= SequenceHandlerChain<string, object>.CreateChainFromType<IUserPropertyHandler>();
        
        private IHandler<ITrackingEvent> _eventModifier;
        private IHandler<ITrackingEvent> EventModifier => _eventModifier ??= SequenceHandlerChain<ITrackingEvent>.CreateChainFromType<ITrackingEventModifyHandler>();

        private IHandler<ITrackingEvent> _eventHandler;
        private IHandler<ITrackingEvent> EventHandler => _eventHandler ??= SequenceHandlerChain<ITrackingEvent>.CreateChainFromType<ITrackingHandler>();

        public void Track(TrackingScreen screen) => Track(new GameScreenParams(screen));

        public void Track(ITrackingEvent e)
        {
            EventModifier.Handle(e);
            EventHandler.Handle(e);
            _LogEvent(e);
            if (e is GameScreenParams ee) TrackingEvents.OnGameScreenTrackedEvent.Invoke(ee);
        }

        [Conditional("LOG_INFO")]
        private void _LogEvent(ITrackingEvent e)
        {
            if (e is IConvertableEvent convertableEvent)
                this.LogInfo(SharedLogTag.Tracking, "eventName", e.EventName, nameof(e), convertableEvent.ToConvertableEvent());
            else
                this.LogInfo(SharedLogTag.Tracking, "eventName", e.EventName, nameof(e), e);
        }

        public void SetUserProperty(string key, object val)
        {
            this.LogInfo(nameof(key), key, nameof(val), val);
            UserPropertyHandler?.Handle(key, val);
        }
    }
}