using Shared.Core.Async;
using UnityEngine;

namespace Shared.SharedReport.Version
{
    public abstract class AbstractVersionGetter : IVersionGetter
    {
        public MonoBehaviour CoroutineBehaviour { get; }
        
        protected AbstractVersionGetter(MonoBehaviour coroutineBehaviour)
        {
            CoroutineBehaviour = coroutineBehaviour;
        }

        public abstract IResultAsyncOperation<VersionEntity> Get();
    }
}