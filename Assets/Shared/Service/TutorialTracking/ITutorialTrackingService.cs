namespace Shared.Service.TutorialTracking
{
    public interface ITutorialTrackingService
    {
        void StartTutorial();
        void CompleteStep();
        void CompleteTutorial();
        void SkipTutorial();
    }
}