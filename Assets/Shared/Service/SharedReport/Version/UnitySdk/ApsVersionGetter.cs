#if APS
using Shared.Core.Async;
using UnityEngine;

namespace Shared.SharedReport.Version.UnitySdk
{
    public class ApsVersionGetter : AbstractVersionGetter
    {
        public ApsVersionGetter(MonoBehaviour coroutineBehaviour) : base(coroutineBehaviour)
        {
        }

        public override IResultAsyncOperation<VersionEntity> Get()
        {
            var operation = new ResultAsyncOperationImpl<VersionEntity>();
            var entity = new VersionEntity("aps");
            entity.AddUnityPluginVersion(AmazonConstants.VERSION);
            operation.SuccessWithResult(entity);
            return operation;
        }
    }
}
#endif