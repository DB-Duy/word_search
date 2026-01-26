using Shared.Core.IoC;
using TMPro;
using UnityEngine;

namespace Shared.View.Version
{
    [DisallowMultipleComponent]
    public class VersionText : IoCMonoBehavior, ISharedUtility
    {
        [SerializeField] private TextMeshProUGUI text;

        private void Start()
        {
            text ??= GetComponent<TextMeshProUGUI>();
            text.text = $"{Application.version}({this.GetVersionCode()})";
        }
    }
}