#if APP_ICON
using Shared.Core.IoC;

namespace Shared.Service.AppIcon
{
    /// <summary>
    /// https://github.com/kyubuns/AppIconChangerUnity
    /// </summary>
    [Service]
    public class AppIconService : IAppIconService
    {
        public bool SupportsAlternateIcons()
        {
            return AppIconChanger.iOS.SupportsAlternateIcons;
        }

        public string GetAppIconName()
        {
            return AppIconChanger.iOS.AlternateIconName;
        }

        public void SetAppIconByName(string iconName)
        {
            AppIconChanger.iOS.SetAlternateIconName(iconName);
        }
    }
}
#endif
