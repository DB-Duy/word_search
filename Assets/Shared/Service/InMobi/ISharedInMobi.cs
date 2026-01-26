using System.Collections.Generic;

namespace Shared.Service.InMobi
{
    public interface ISharedInMobi
    {
        void UpdateGdprConsent(Dictionary<string, object> consentObject);
        
        void SetUSPrivacyString(string usPrivacyString);
    }
}