using Shared.Core.Async;
using Shared.Core.IoC;
using Shared.Service.Tracking;
using Shared.Service.Tracking.Common;
using Shared.Tracking.ErrorTracking;
using Shared.Tracking.Models.Game;
using Shared.Utils;
using UnityEngine;
using Zenject;

namespace Shared.Service.ErrorTracking
{
    [Service]
    public class ErrorTrackingService : IErrorTrackingService
    {
        private const string Tag = "ErrorTrackingService";

        [Inject] private ITrackingService _trackingService;

        public bool IsInitialized { get; private set; } = false;

        private readonly IValidator _errorValidator = new Shared.Tracking.ErrorTracking.Validator();
        private readonly IValidator _exceptionValidator = new Shared.Tracking.ErrorTracking.Validator();

        public IAsyncOperation Initialize()
        {
            SharedLogger.Log($"{Tag}->Initialize");
            if (IsInitialized) return new SharedAsyncOperation().Success();
            IsInitialized = true;
            Application.logMessageReceived += _HandleLog;
            return new SharedAsyncOperation().Success();
        }
        
        private void _HandleLog(string logString, string stackTrace, LogType type)
        {
            if (type == LogType.Error)
            {
                var isValid = _errorValidator.Validate(logString);
                if (isValid) _trackingService.Track(new GameErrorParams(logString, severity: LogSeverity.Error));
            }
            else if (type == LogType.Exception)
            {
                var isValid = _exceptionValidator.Validate(logString);
                if (isValid) _trackingService.Track(new GameErrorParams(logString, severity: LogSeverity.Exception));
            }
        }
    }
}