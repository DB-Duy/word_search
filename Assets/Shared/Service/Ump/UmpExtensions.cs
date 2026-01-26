#if USING_UMP
using GoogleMobileAds.Ump.Api;

namespace Shared.Service.Ump
{
    public static class UmpExtensions
    {
        public static string ToInfoString(this FormError error)
        {
            return StringUtils.ToJsonString(nameof(error.ErrorCode), error?.ErrorCode, nameof(error.Message), error?.Message);
        }
    }
}
#endif