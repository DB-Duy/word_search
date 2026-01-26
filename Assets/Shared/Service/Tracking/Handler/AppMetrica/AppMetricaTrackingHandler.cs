#if APPMETRICA
using System.Collections.Generic;
using Shared.Core.Handler;
using Shared.Core.IoC;
using Shared.Service.Tracking.Handler.AppMetrica.Internal;
using Shared.Tracking.Templates;
using UnityEngine;

namespace Shared.Service.Tracking.Handler.AppMetrica
{
    [Component]
    public class AppMetricaTrackingHandler : ITrackingHandler, ISharedUtility
    {
        private IHandler<ITrackingEvent> _appMetricaEventHandler;
        public IHandler<ITrackingEvent> AppMetricaEventHandler => _appMetricaEventHandler ??= SequenceHandlerChain<ITrackingEvent>.CreateChainFromType<IAppMetricaTrackingHandler>();
        
        private readonly Queue<ITrackingEvent> _eventQueue = new();

        public void Handle(ITrackingEvent e)
        {
            if (Application.isEditor) return;
            
            if (!Io.AppMetrica.AppMetrica.IsActivated())
            {
                _eventQueue.Enqueue(e);
                return;
            }
            
            if (_eventQueue.Count == 0)
            {
                AppMetricaEventHandler?.Handle(e);
                return;
            }

            _eventQueue.Enqueue(e);
            var max = _eventQueue.Count >  100 ? 100 : _eventQueue.Count;
            for (var i = 0; i < max; i++)
            {
                var oldEvent = _eventQueue.Dequeue();
                AppMetricaEventHandler?.Handle(oldEvent);
            }
        }
    }
}
#endif