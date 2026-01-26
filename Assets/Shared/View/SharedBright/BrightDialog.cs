using Shared.Core.IoC;
using Shared.Service.SharedBright;
using UnityEngine;
using Zenject;

namespace Shared.View.SharedBright
{
    [DisallowMultipleComponent]
    public class BrightDialog : IoCMonoBehavior
    {
        [Inject(Optional = true)] private IBrightService _service;

        private void OnEnable()
        {
            _service?.NotifyConsentShown();
        }

        public void OptOut()
        {
            _service?.OptOut();
        }

        public void ExternalOptIn()
        {
            _service?.ExternalOptIn();
        }
        
        public void OpenBrightUrl()
        {
            _service?.OpenBrightUrl();
        }

        public void OpenPrivacyUrl()
        {
            _service?.OpenPrivacyUrl();
        }

        public void OpenUserLicenseUrl()
        {
            _service?.OpenUserLicenseUrl();
        }
    }
}