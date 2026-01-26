#if GOOGLE_PLAY
namespace Shared.Referrer
{
    public class InstallReferrerResponse
    {
        public const int SERVICE_DISCONNECTED = -1;
        public const int OK = 0;
        public const int SERVICE_UNAVAILABLE = 1;
        public const int FEATURE_NOT_SUPPORTED = 2;
        public const int DEVELOPER_ERROR = 3;
        public const int PERMISSION_ERROR = 4;
    }
}
#endif