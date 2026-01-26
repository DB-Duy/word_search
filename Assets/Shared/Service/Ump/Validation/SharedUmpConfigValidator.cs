using Shared.Core.IoC;
using Shared.Repository.Ump;
using Shared.Utilities;
using Shared.Utils;
using Zenject;

namespace Shared.Service.Ump.Validation
{
    [Component]
    public class SharedUmpConfigValidator : IUmpValidator, ISharedUtility, ISharedLogTag
    {
        [Inject] private UmpConfigRepository _configRepository;

        public bool Validate()
        {
            var umpConfig = _configRepository.Get();
            this.LogInfo(nameof(umpConfig), umpConfig);
            return umpConfig.IsEnable;
        }

        public string LogTag => SharedLogTag.Ump;
    }
}