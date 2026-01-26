using System.Collections.Generic;

namespace Shared.Tracking.Utils
{
    public interface ISpecificEndgameParamsBuilder
    {
        Dictionary<string, object> Build();
    }
}