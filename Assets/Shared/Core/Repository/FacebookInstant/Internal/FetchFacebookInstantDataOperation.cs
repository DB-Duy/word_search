using System;
using Shared.Core.Async;

namespace Shared.Repository.FacebookInstant.Internal
{
    public class FetchFacebookInstantDataOperation : IFetchFacebookInstantDataOperation
    {
        public Action OnCompleteEvent { get; set; }
        public bool IsComplete { get; private set; }
        public bool IsSuccess { get; private set; }
        public string FailReason { get; private set; }
        public string JsonData { get; private set; }
        
        public void Success(string jsonData)
        {
            IsComplete = true;
            IsSuccess = true;
            JsonData = jsonData;
        }

        public IAsyncOperation Success()
        {
            throw new System.NotImplementedException();
        }

        public IAsyncOperation Fail(string reason)
        {
            IsComplete = true;
            IsSuccess = false;
            FailReason = reason;
            JsonData = null;
            return this;
        }

        public IAsyncOperation Fail(string reason, params object[] p)
        {
            throw new NotImplementedException();
        }
    }
}