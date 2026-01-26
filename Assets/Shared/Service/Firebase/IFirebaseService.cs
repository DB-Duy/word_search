using Shared.Core.Handler;
using Shared.Core.Handler.Corou.Initializable;

namespace Shared.Service.Firebase
{
    public interface IFirebaseService : IInitializable
    {
#if FIREBASE
        IHandler<global::Firebase.FirebaseApp> PostInitHandler { get; set; }
#endif
        string AnalyticsInstanceId { get; }
    }
}