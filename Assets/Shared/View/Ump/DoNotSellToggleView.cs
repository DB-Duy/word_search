using Shared.Core.IoC;
using Shared.Service.Ump;
using Shared.Utils;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Shared.View.Ump
{
    [RequireComponent(typeof(Toggle))]
    [DisallowMultipleComponent]
    public class DoNotSellToggleView : IoCMonoBehavior
    { 
        private Toggle _toggleDoNotSell;
        
        [Inject(Optional = true)] private IUmpService _umpService;

        private void Start()
        {
            _toggleDoNotSell ??= GetComponent<Toggle>();
            _toggleDoNotSell.SetIsOnWithoutNotify(_umpService.IsTurnOn());
            _toggleDoNotSell.onValueChanged.AddListener((on) =>
            {
                if (Application.isEditor) return;
                if (_umpService == null)
                {
                    this.LogError(SharedLogTag.Ump, "ignore", "umpService == null");
                    return;
                }

                _umpService.UpdateUsPrivacyValue(on);
            });
        }
    }
}