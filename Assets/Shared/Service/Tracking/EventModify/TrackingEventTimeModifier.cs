using System;
using Shared.Core.IoC;
using Shared.Tracking.Templates;

namespace Shared.Service.Tracking.EventModify
{
    [Component]
    public class TrackingEventTimeModifier : ITrackingEventModifyHandler
    {
        private readonly string _paramName = "event_timestamp";

        public void Handle(ITrackingEvent e)
        {
            if (e is not IExParamsEvent ee) return;
            var currentMilliseconds = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            ee.AddParams(_paramName, currentMilliseconds);
        }
    }
}