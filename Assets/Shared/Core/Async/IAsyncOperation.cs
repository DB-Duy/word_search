using System;

namespace Shared.Core.Async
{
    public interface IAsyncOperation
    {
        Action OnCompleteEvent { get; set; }
        bool IsComplete { get; }
        bool IsSuccess { get; }
        string FailReason { get; }

        IAsyncOperation Success();
        IAsyncOperation Fail(string reason);
        IAsyncOperation Fail(string reason, params object[] p);
    }
    
    public interface IAsyncOperation<T> : IAsyncOperation
    {
        T Data { get; set; }
    }
}