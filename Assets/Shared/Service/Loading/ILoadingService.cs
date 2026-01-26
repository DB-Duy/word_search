using System;
using Shared.Core.Async;
using Shared.Core.Handler.Corou.Initializable;

namespace Shared.Service.Loading
{
    public interface ILoadingService : IInitializable
    {
        IAsyncOperation<LoadOperation> Load();
        Action OnLoadComplete { get; set; }
    }
}