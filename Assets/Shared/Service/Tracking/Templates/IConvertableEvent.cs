using System.Collections.Generic;

namespace Shared.Tracking.Templates
{
    public interface IConvertableEvent
    {
        Dictionary<string, object> ToConvertableEvent();
    }
}