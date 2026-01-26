using Shared.Core.IoC;
using Shared.Service.Ump;
using Shared.Utils;
using UnityEngine;

namespace Shared.View.Ump
{
    [DisallowMultipleComponent]
    public class UmpDebugButton : MonoBehaviour, IIoC
    {
        private const string Tag = "DebugConsentInfoButton";
        
        public void DebugUmp()
        {
#if USING_UMP
            var e = this.Resolve<IUmpService>().Get();
            SharedLogger.LogJson(SharedLogTag.Ump, $"{Tag}->DebugUmp", nameof(e), e);
#endif
        }
    }
}