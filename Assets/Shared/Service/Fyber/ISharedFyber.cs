namespace Shared.Service.Fyber
{
    public interface ISharedFyber
    {
        void SetGdprConsent(bool value);
        void SetGdprConsentString(string consentString);
        void SetUsPrivacyString(string value);

        void ClearUsPrivacyString();
        void ClearGdprConsentData();
    }
}