using Shared.Core.View.Binding;
using Shared.Core.View.Screen;
using Shared.Repository.UIExample;
using TMPro;
using UnityEngine;

namespace Shared.View.UIExample
{
    public class UIExampleScreen : MonoBehaviour, IUIScreen, ISharedUtility
    {
        [SerializeField] private TextMeshProUGUI textLabel;
        
        private void Awake()
        {
            this.Bind<UIExampleIntRepository>(textLabel);
        }
    }
}