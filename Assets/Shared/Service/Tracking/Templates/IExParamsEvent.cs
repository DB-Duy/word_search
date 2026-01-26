
using System.Collections.Generic;

namespace Shared.Tracking.Templates
{
    public interface IExParamsEvent
    {
        Dictionary<string, object> ExParams { get; }
        void AddParams(string paramName1, object paramValue1);
    }
}