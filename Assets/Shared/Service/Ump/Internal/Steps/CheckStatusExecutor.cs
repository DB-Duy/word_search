#if USING_UMP
using GoogleMobileAds.Ump.Api;
using Shared.Core.Async;
using Shared.Service.Schedule.Internal;
using Shared.Utilities;
using Shared.Utils;

namespace Shared.Service.Ump.Internal.Steps
{
    public class CheckStatusExecutor : IStepExecutor, ISharedUtility, ISharedLogTag
    {
        private IAsyncOperation _operation;
        
        public IAsyncOperation Execute(UmpProcessInfo i)
        {
            if (_operation != null) return _operation;
            _operation = new SharedAsyncOperation();
            var scheduleTimeOutTask = new SharedRunner("UmpTimeOut", i.TimeOutInSeconds, () =>
            {
                this.LogError("f", "Scheduler", "reason", "Timeout");
                _operation?.Fail("TimeOut");
            });
            this.Schedule(scheduleTimeOutTask);
            ConsentInformation.Update(i.ConsentRequestParameters, (formError) =>
            {
                // Handle Timeout case
                this.LogInfo("f", "OnConsentInfoUpdateCallback", nameof(formError), formError?.ToInfoString());
                if (_operation.IsComplete)
                {
                    this.LogError("f", "OnConsentInfoUpdateCallback", "reason", "Timeout");
                    return;
                }
                this.RemoveSchedule(scheduleTimeOutTask);

                var errorMessage = string.Empty;
                if (formError != null) errorMessage = $"{formError.ErrorCode} - {formError.Message}";
                // If the error is null, the consent information state was updated.
                // You are now ready to check if a form is available.
                if (!ConsentInformation.IsConsentFormAvailable()) errorMessage = "!ConsentInformation.IsConsentFormAvailable()";
            
                if (!string.IsNullOrEmpty(errorMessage))
                {
                    this.LogError("f", "OnConsentInfoUpdateCallback", nameof(errorMessage), errorMessage);
                    _operation.Fail($"OnConsentInfoUpdateCallback: {errorMessage}");
                    return;
                }
                _operation.Success();
            });
            return _operation;
        }

        public string LogTag => SharedLogTag.Ump;
    }
}
#endif