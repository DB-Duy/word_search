using Shared.Core.IoC;
using Shared.Entity.Config;
using Shared.Utils;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Shared.View.Privacy
{
    [RequireComponent(typeof(Button))]
    public class PrivacyButton : IoCMonoBehavior, ISharedUtility
    {
        [Inject] private IConfig _config;

        [SerializeField] private Button button;

        private void Start()
        {
            button.onClick.AddListener(() =>
            {
                this.LogInfo(SharedLogTag.Privacy, "f", nameof(Start), nameof(_config.PrivacyUrl), _config.PrivacyUrl);
                Application.OpenURL(_config.PrivacyUrl);    
            });
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            button = GetComponent<Button>();
        }
#endif
    }
}
