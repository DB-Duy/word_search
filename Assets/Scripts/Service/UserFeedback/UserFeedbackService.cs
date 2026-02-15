
using Shared;
using Shared.Core.IoC;
using Shared.Service.Audio;
using Zenject;

namespace Service.UserFeedback
{
    [Service]
    public class UserFeedbackService : ISharedUtility, IInitializable
    {
        private const string Tag = "UserFeedbackService";
        public static UserFeedbackService Instance { get; private set; }

        [Inject] private AudioService _audioService;

        public void Initialize()
        {
            if (Instance != null)
            {
                return;
            }
            Instance = this;
        }
    }
}