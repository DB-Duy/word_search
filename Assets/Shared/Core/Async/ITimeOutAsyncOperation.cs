namespace Shared.Core.Async
{
    public interface ITimeOutAsyncOperation : IAsyncOperation
    {
        bool IsTimeOut { get; }
        ITimeOutAsyncOperation StartTimeOut();
    }
}