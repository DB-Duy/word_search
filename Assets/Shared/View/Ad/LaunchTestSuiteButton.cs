using Shared.Core.IoC;
using Shared.Service.Ads;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Shared.View.Ad
{
    [RequireComponent(typeof(Button))]
    public class LaunchTestSuiteButton : IoCMonoBehavior
    {
        [Inject] private IAdService _adService;
        [SerializeField] private Button button;

// #if HUAWEI
//                 AnyThinkAds.Api.ATSDKAPI.showDebuggerUI();
// #elif GOOGLE_PLAY || APPSTORE || APPSTORE_CHINA
//         SharedCore.Instance.AdController.LaunchTestSuite();
// #endif
        private void Start()
        {
            if (button == null) button = GetComponent<Button>();
            button.onClick.AddListener(() => _adService.LaunchTestSuite());
        }
    }
}