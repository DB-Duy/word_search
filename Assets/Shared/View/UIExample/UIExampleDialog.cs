using Shared.Core.View.Binding;
using Shared.Core.View.Dialog;
using Shared.Repository.UIExample;
using TMPro;
using UnityEngine;

namespace Shared.View.UIExample
{
    public class UIExampleDialog : MonoBehaviour, IUIDialog, ISharedUtility
    {
        [SerializeField] private TextMeshProUGUI textLabel;
        
        private void Awake()
        {
            this.Bind<UIExampleIntRepository>(textLabel);
        }
    }
}