using Shared.Core.Handler.Corou.Initializable;

namespace Shared.Service.SharedBright
{
    public interface IBrightService : IInitializable
    {   
        bool Validate();
        bool IsUnlocked();
        
        void NotifyConsentShown();
        bool IsOptIn();
        
        void OptOut();
        void OptOutManual();
        void ExternalOptIn();

        void OpenBrightUrl();
        void OpenPrivacyUrl();
        void OpenUserLicenseUrl();

        void LearnMore();
    }
}