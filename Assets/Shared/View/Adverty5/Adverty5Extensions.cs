#if ADVERTY_5
using Adverty5.AdPlacements;

namespace Shared.View.Adverty5
{
    public static class Adverty5Extensions
    {
        public static string ToInfoString(this AdPlacement p)
        {
            return StringUtils.ToJsonString(nameof(p.name), p.name, nameof(p.Id), p.Id, nameof(p.IsActive), p.IsActive, nameof(p.AllowVideo), p.AllowVideo);
        }
    }
}
#endif