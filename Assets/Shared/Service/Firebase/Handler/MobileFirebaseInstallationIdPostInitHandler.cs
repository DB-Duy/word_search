#if FIREBASE
using Firebase;
using Firebase.Installations;
using Shared.Core.Handler;
using Shared.Utils;

namespace Shared.Service.Firebase.Handler
{
    public class MobileFirebaseInstallationIdPostInitHandler : IHandler<FirebaseApp>
    {
        private const string Tag = "FirebaseInstallationIdPostInitHandler";
        
        public void Handle(FirebaseApp app) => _DebugInstallationId(app);
        
        private static async void _DebugInstallationId(FirebaseApp app)
        {
            var firebaseInstallations = FirebaseInstallations.GetInstance(app);
            var idTask = firebaseInstallations.GetIdAsync();
            await idTask;
            SharedLogger.LogJson(SharedLogTag.Firebase, $"{Tag}->_DebugInstallationId", nameof(idTask), idTask.ToDict());
        }
    }
}
#endif