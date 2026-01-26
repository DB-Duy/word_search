using Shared.Core.Async;

namespace Shared.Service.StoreRating
{
    public interface IStoreRatingService
    {
        bool Validate();
        
        void IncreaseShownCountByOne();
        IAsyncOperation Rate();
    }
}