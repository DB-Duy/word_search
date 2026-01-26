using Shared.Core.IoC;
using Shared.Entity.Config;
using Shared.Utils;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Shared.View.Privacy
{
    public class TermOfUseButton : IoCMonoBehavior
    {
        private const string Tag = "TermOfUseButton";

        [Inject] private IConfig _config;
        [SerializeField] private Button button;

#if UNITY_EDITOR
        private void OnValidate()
        {
            button = GetComponent<Button>();
        }
#endif

        private void Start()
        {
            button ??= GetComponent<Button>();
            button.onClick.AddListener(() =>
            {
                SharedLogger.LogJson(SharedLogTag.Privacy, $"{Tag}->Start.button.onClick", nameof(_config.TermOfUseUrl), _config.TermOfUseUrl);
                Application.OpenURL(_config.TermOfUseUrl);
            });
        }
    }
}
