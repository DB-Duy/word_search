#if APPMETRICA
using Io.AppMetrica.Profile;
using Shared.Core.Handler;
using Shared.Core.IoC;
using Shared.Utils;

namespace Shared.Service.Tracking.Handler.AppMetrica
{
    [Component]
    public class AppMetricaUserPropertyHandler : IHandler<string, object>, IUserPropertyHandler, ISharedUtility
    {
        public void Handle(string k, object v)
        {
            var userProfile = new UserProfile();
            switch (v)
            {
                case string s:
                    this.LogInfo(SharedLogTag.UserPropertyNAppMetrica, nameof(k), k, nameof(v), v, "type", "string");
                    userProfile.Apply(Attribute.CustomString(k).WithValue(s));
                    break;
                case int i:
                    this.LogInfo(SharedLogTag.UserPropertyNAppMetrica, nameof(k), k, nameof(v), v, "type", "int");
                    userProfile.Apply(Attribute.CustomNumber(k).WithValue(i));
                    break;
                default:
                    this.LogError(SharedLogTag.UserPropertyNAppMetrica, nameof(k), k, nameof(v), v, "type", "unknown");
                    return;
            }
            Io.AppMetrica.AppMetrica.ReportUserProfile(userProfile);
        }
    }
}
#endif