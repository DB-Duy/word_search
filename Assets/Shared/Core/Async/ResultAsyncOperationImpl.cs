using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Shared.Utils;

namespace Shared.Core.Async
{
    public class ResultAsyncOperationImpl<T> : IResultAsyncOperation<T>
    {
        public Action OnCompleteEvent { get; set; }
        public bool IsComplete { get; private set; }
        public bool IsSuccess { get; private set; }
        public string FailReason { get; private set; }
        public T Result { get; private set; }

        public IResultAsyncOperation<T> SuccessWithResult(T result)
        {
            IsComplete = true;
            IsSuccess = true;
            Result = result;
            OnCompleteEvent?.Invoke();
            return this;
        }

        public IAsyncOperation Success()
        {
            IsComplete = true;
            IsSuccess = true;
            OnCompleteEvent?.Invoke();
            return this;
        }

        public IAsyncOperation Fail(string reason)
        {
            IsComplete = true;
            IsSuccess = false;
            FailReason = reason;
            OnCompleteEvent?.Invoke();
            return this;
        }

        public IAsyncOperation Fail(string reason, params object[] p)
        {
            var dict = new Dictionary<string, object>();
            var length = p.Length % 2 == 0 ? p.Length : p.Length - 1; 
            for (var i = 0; i < length; i= i + 2)
                dict.Upsert(p[i].ToString(), p[i + 1]);
            return Fail($"{reason} {JsonConvert.SerializeObject(dict)}");
        }
    }
}