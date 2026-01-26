using System.Collections.Generic;
using Shared.Core.Async;
using Shared.Utils;
using UnityEngine;

namespace Shared.SharedReport.Version.Android
{
    public class AndroidSdkVersionGetter : AbstractVersionGetter
    {
        private string Name { get; }
        private readonly Dictionary<string, string> _config = new();

        public AndroidSdkVersionGetter(MonoBehaviour coroutineBehaviour, string name, params string[] config) : base(coroutineBehaviour)
        {
            Name = name;
            _config.AddRange(config);
        }

        public void AddConfig(string name, string accessPoint)
        {
            _config.Upsert(name, accessPoint);
        }

        public override IResultAsyncOperation<VersionEntity> Get()
        {
            var operation = new ResultAsyncOperationImpl<VersionEntity>();
            var entity = new VersionEntity(Name);

            foreach (var kv in _config)
            {
                entity.AddVersion(kv.Key, AndroidVersionUtils.GetVersionViaAccessPoint(kv.Value));    
            }
            operation.SuccessWithResult(entity);
            return operation;
        }
    }
}