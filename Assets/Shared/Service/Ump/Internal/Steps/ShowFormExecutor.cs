#if USING_UMP
using Shared.Core.Async;
using Shared.Service.Tracking;
using Shared.Service.Tracking.Common;
using Shared.Tracking;
using Shared.Utilities;
using Shared.Utils;

namespace Shared.Service.Ump.Internal.Steps
{
    public class ShowFormExecutor : IStepExecutor, ISharedUtility, ISharedLogTag
    {
        private const string Tag = "ShowFormExecutor";
        private IAsyncOperation _operation;
        
        public IAsyncOperation Execute(UmpProcessInfo i)
        {
            if (_operation != null) return _operation;
            _operation = new SharedAsyncOperation();
            i.ConsentForm.Show((formError) =>
            {
                this.LogInfo("f", "OnDismissed", nameof(formError), formError.ToInfoString());
                var errorMessage = string.Empty;
                if (formError != null) errorMessage = $"{formError.ErrorCode} - {formError.Message}";
            
                if (!string.IsNullOrEmpty(errorMessage))
                {
                    this.LogError("f", "OnDismissed", nameof(errorMessage), errorMessage);
                    _operation.Fail($"{Tag}->OnDismissed: {errorMessage}");
                    return;
                }

                _operation.Success();
            });
            this.Track(TrackingScreen.Ump);
            
            return _operation;
        }

        public string LogTag => SharedLogTag.Ump;
    }
}
#endif