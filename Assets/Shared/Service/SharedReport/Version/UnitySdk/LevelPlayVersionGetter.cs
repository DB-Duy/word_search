#if LEVEL_PLAY
using Shared.Core.Async;
using UnityEngine;

namespace Shared.SharedReport.Version.UnitySdk
{
    public class LevelPlayVersionGetter : AbstractVersionGetter
    {
        public LevelPlayVersionGetter(MonoBehaviour coroutineBehaviour) : base(coroutineBehaviour)
        {
        }

        public override IResultAsyncOperation<VersionEntity> Get()
        {
            var operation = new ResultAsyncOperationImpl<VersionEntity>();
            var entity = new VersionEntity("levelplay");
            entity.AddUnityPluginVersion(IronSource.pluginVersion());
            operation.SuccessWithResult(entity);
            return operation;
        }
    }
}
#endif