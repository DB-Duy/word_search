using Shared.Core.Async;
using UnityEngine;

namespace Shared.SharedReport.Version
{
    public interface IVersionGetter
    {
        MonoBehaviour CoroutineBehaviour { get; }
        IResultAsyncOperation<VersionEntity> Get();
    }
}