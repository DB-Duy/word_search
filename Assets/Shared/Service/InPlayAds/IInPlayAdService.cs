using Shared.Core.Handler.Corou.Initializable;

namespace Shared.Service.InPlayAds
{
    public interface IInPlayAdService : IInitializable
    {
        bool ValidateByValidators(string placement);
        void RemoveAds();
    }
}