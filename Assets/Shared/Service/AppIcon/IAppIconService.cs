namespace Shared.Service.AppIcon
{
    public interface IAppIconService
    {
        /// <summary>
        /// Check to see if the icon can be changed
        /// https://developer.apple.com/documentation/uikit/uiapplication/2806815-supportsalternateicons
        /// </summary>
        bool SupportsAlternateIcons();
        /// <summary>
        /// Check the current icon name (null is the default)
        /// https://developer.apple.com/documentation/uikit/uiapplication/2806808-alternateiconname
        /// </summary>
        string GetAppIconName();
        /// <summary>
        /// Set the icon (null to restore the default)
        /// https://developer.apple.com/documentation/uikit/uiapplication/2806818-setalternateiconname
        /// </summary>
        /// <param name="iconName"></param>
        void SetAppIconByName(string iconName);
    }
}

