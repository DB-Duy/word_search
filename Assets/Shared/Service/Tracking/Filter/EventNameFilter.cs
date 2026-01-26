using System.Collections.Generic;
using Shared.Tracking.Templates;
using Shared.Utils;

namespace Shared.Service.Tracking.Filter
{
    public class EventNameFilter : IEventFilter
    {
        // private const string Tag = "EventNameFilter";
        private readonly HashSet<string> _ignoreNames = new();

        public EventNameFilter(params string[] eventNames)
        {
            _ignoreNames.AddRange(eventNames);
        }

        public bool Validate(ITrackingEvent e) => !_ignoreNames.Contains(e.EventName);
    }
}