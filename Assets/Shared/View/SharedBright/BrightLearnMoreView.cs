using UnityEngine;
using UnityEngine.UI;

namespace Shared.View.SharedBright
{
    [RequireComponent(typeof(Button))]
    [DisallowMultipleComponent]
    public class BrightLearnMoreView : MonoBehaviour
    {
        private const string LearnMoreURL = "https://bright-sdk.com/users#learn-more-about-bright-sdk-web-indexing";

        private void Start()
        {
            var btn = GetComponent<Button>();
            btn.onClick.AddListener(_LearnMore);
        }

        private static void _LearnMore()
        {
            Application.OpenURL(LearnMoreURL);
        }
    }
}