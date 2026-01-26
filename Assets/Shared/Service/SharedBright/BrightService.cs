#if BRIGHT_DATA_SDK

using System.IO;
using Brdsdk;
using Shared.Core.Async;
using Shared.Core.IoC;
using Shared.Repository.SharedBright;
using Shared.Utils;
using UnityEngine;
using Zenject;

namespace Shared.Service.SharedBright
{
    [Service]
    public class BrightService : IBrightService, ISharedUtility
    {
        private const string BrightURL = "https://brightdata.com";
        private const string BrightPrivacyURL = "https://bright-sdk.com/privacy-policy";
        private const string UserLicenseURL = "https://bright-sdk.com/eula";
        private const string LearnMoreURL = "https://bright-sdk.com/users#learn-more-about-bright-sdk-web-indexing";
        
        [Inject(Optional = true)] private IBrightValidator  _validator;
        [Inject] private BrightSdkConfigRepository _brightConfigRepository;
        
        public bool IsInitialized { get; private set; }
        private IAsyncOperation _initOperation;
        public IAsyncOperation Initialize()
        {
            if (_initOperation != null) return _initOperation;
            _initOperation = new SharedAsyncOperation();
            
            BrdsdkBridge.set_on_sdk_ready_callback(_OnSdkReady);
            // BrdsdkBridge.set_on_choice_change_callback(_OnChoiceChanged);
            var appicon = Path.Combine(Application.streamingAssetsPath, "appicon.png");
            BrdsdkBridge.init(
                benefit: "To support this app", 
                agree_btn: "I Agree", 
                disagree_btn: "I disagree",
                opt_out_instructions: "Just a test text of opt-out instructions.",
                appicon: appicon, 
                on_choice_callback: _OnChoiceChanged,
                skip_consent: true);
            IsInitialized = true;
            _initOperation.Success();
            return _initOperation;
        }
        
        private void _OnSdkReady()
        {
            this.LogInfo(SharedLogTag.Bright, "f", nameof(_OnSdkReady));
        }
        
        private void _OnChoiceChanged(int choice)
        {
            this.LogInfo(SharedLogTag.Bright, nameof(choice), choice.ToString());
        }

        public bool Validate()
        {
            return _validator?.Validate() ?? false;
        }

        public bool IsUnlocked()
        {
            var c = _brightConfigRepository?.Get(); 
            return c?.Unlocked ?? false;
        }

        public void NotifyConsentShown()
        {
            this.LogInfo(SharedLogTag.Bright, "call", "BrdsdkBridge.notify_consent_shown();");
            BrdsdkBridge.notify_consent_shown();
        }

        public bool IsOptIn()
        {
            var choice = BrdsdkBridge.current_choice();
            this.LogInfo(SharedLogTag.Bright, nameof(choice), choice.ToString());
            return choice == Choice.Peer;
        }

        public void OptOut()
        {
            this.LogInfo(SharedLogTag.Bright, "f", nameof(OptOut));
            BrdsdkBridge.opt_out();
        }

        public void OptOutManual()
        {
            this.LogInfo(SharedLogTag.Bright, "f", nameof(OptOutManual));
            BrdsdkBridge.opt_out(ChoiceTriggerType.Manual);
        }

        public void ExternalOptIn()
        {
            this.LogInfo(SharedLogTag.Bright, "f", nameof(ExternalOptIn));
            BrdsdkBridge.external_opt_in();
        }
        
        public void OpenBrightUrl()
        {
            Application.OpenURL(BrightURL);
        }

        public void OpenPrivacyUrl()
        {
            Application.OpenURL(BrightPrivacyURL);
        }

        public void OpenUserLicenseUrl()
        {
            Application.OpenURL(UserLicenseURL);
        }

        public void LearnMore()
        {
            Application.OpenURL(LearnMoreURL);
        }
    }
}
#endif