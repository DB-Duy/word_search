using Shared.Core.Handler.Corou.Initializable;

namespace Shared.Service.Session
{
    public interface ISessionService : IInitializable
    {
        long SessionId { get; }
    }
}