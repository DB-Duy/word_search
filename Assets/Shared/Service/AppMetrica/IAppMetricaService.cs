#if APPMETRICA
using Shared.Core.Handler.Corou.Initializable;

namespace Shared.Service.AppMetrica
{
    public interface IAppMetricaService : IInitializable
    {
        string ProfileId { get; }
    }
}
#endif
