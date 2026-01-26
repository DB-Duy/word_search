using Shared.Core.View.FloatingMessage;
using Shared.Utilities.SharedBehaviour;
using TMPro;
using UnityEngine;

namespace Shared.View.FloatingMessage
{
    [DisallowMultipleComponent]
    public class FloatingMessage : SharedMonoBehaviour, IUIFloatingMessage
    {
        [SerializeField] private TextMeshProUGUI messageText;
        private TextMeshProUGUI TextLabel => messageText ??= GetComponent<TextMeshProUGUI>();

        public string Text
        {
            set => TextLabel.text = value;
        }
    }
}