#if USING_UMP
using System.Collections.Generic;
using GoogleMobileAds.Ump.Api;
using Shared.Core.Async;
using Shared.Utilities;
using Shared.Utils;

namespace Shared.Service.Ump.Internal.Steps
{
    public class LoadFormExecutor : IStepExecutor, ISharedLogTag, ISharedUtility
    {
        private List<ConsentStatus> _requireStatus = new();

        public LoadFormExecutor(bool isEdit = false)
        {
            _requireStatus.Add(ConsentStatus.Required);
            if (isEdit) _requireStatus.Add(ConsentStatus.Obtained);
        }

        private IAsyncOperation _operation;
        public IAsyncOperation Execute(UmpProcessInfo i)
        {
            if (_operation != null) return _operation;
            _operation = new SharedAsyncOperation();
            ConsentForm.Load((consentForm, formError) =>
            {
                this.LogInfo("f", "OnFormLoadCallback", nameof(formError), formError?.ToInfoString());
                var errorMessage = string.Empty;
                if (formError != null) 
                    errorMessage = $"{formError.ErrorCode} - {formError.Message}";
                else if (!_requireStatus.Contains(ConsentInformation.ConsentStatus)) 
                    errorMessage = $"Invalid ConsentInformation.ConsentStatus({ConsentInformation.ConsentStatus.ToString()})";
                if (!string.IsNullOrEmpty(errorMessage))
                {
                    this.LogWarning("f", "OnFormLoadCallback", nameof(errorMessage), errorMessage);
                    _operation.Fail($"OnFormLoadCallback: {errorMessage}");
                    return;
                }
                // The consent form was loaded.
                // Save the consent form for future requests.
                i.ConsentForm = consentForm;
                _operation.Success();
            });
            return _operation;
        }

        public string LogTag => SharedLogTag.Ump;
    }
}
#endif