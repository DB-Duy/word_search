using Shared.Core.Async;

namespace Shared.Repository.FacebookInstant.Internal
{
    public interface IFetchFacebookInstantDataOperation : IAsyncOperation
    {
        string JsonData { get; }
        void Success(string jsonData);
    }
}