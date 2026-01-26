namespace Shared.Core.Async
{
    public interface IResultAsyncOperation<T> : IAsyncOperation
    {
        T Result { get; }
        IResultAsyncOperation<T> SuccessWithResult(T result);
    }
}