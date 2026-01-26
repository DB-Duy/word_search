using Shared.Core.IoC;
using Shared.Service.Tracking;
using Shared.Tracking.Models.Game;
using Shared.Utils;
using Zenject;

namespace Shared.Service.TutorialTracking
{
    [Service]
    public class TutorialTrackingService : ITutorialTrackingService
    {
        private const string Tag = "TutorialTrackingService";
        [Inject] private ITrackingService _trackingService;
        private int _step = 1;

        public void StartTutorial()
        {
            SharedLogger.LogJson(SharedLogTag.Tutorial, $"{Tag}->StartTutorial");
            _step = 1;
            _Track(-1);
        }

        public void CompleteStep()
        {
            SharedLogger.LogJson(SharedLogTag.Tutorial, $"{Tag}->CompleteStep", nameof(_step), _step);
            _Track(_step);
            _step++;
        }

        public void CompleteTutorial()
        {
            SharedLogger.LogJson(SharedLogTag.Tutorial, $"{Tag}->CompleteTutorial", nameof(_step), _step);
            _Track(-2);
        }

        public void SkipTutorial()
        {
            SharedLogger.LogJson(SharedLogTag.Tutorial, $"{Tag}->SkipTutorial", nameof(_step), _step);
            _Track(0);
        }
        
        // ----------------------------------------------------
        private void _Track(int step)
        {
            var e = new TutorialParams(step);
            _trackingService.Track(e);
        }
    }
}