#if HUAWEI && IN_APP_UPDATE
using System.Collections.Generic;
using System.Diagnostics;
using HmsPlugin;
using Newtonsoft.Json;
using Shared.Common;
using Shared.RemoteConfig;
using Shared.PlayerPrefsRepository.RemoteConfig;
using Shared.Utils;

namespace Shared.InAppUpdate
{
    /// <summary>
    /// https://evilminddevs.gitbook.io/hms-unity-plugin/release-notes
    /// https://evilminddevs.gitbook.io/hms-unity-plugin/kits-and-services/game-service/guides-and-references
    /// https://github.com/EvilMindDevs/hms-unity-plugin/blob/master/Assets/Huawei/Demos/Game/GameDemoManager.cs
    /// </summary>
    public class HuaweiInAppUpdateController : IInAppUpdateService
    {
        private const string TAG = "HuaweiInAppUpdateController";
        
        private IRemoteConfigRepository<InAppUpdateConfig> _remoteConfigRepository;
        private InAppUpdateConfig _remoteConfig;
        

        private IAsyncOperation _downloadOperation;

        public IInAppUpdateService SetUp(IRemoteConfigRepository<InAppUpdateConfig> remoteConfig)
        {
            _remoteConfigRepository = remoteConfig;
            HMSGameServiceManager.Instance.OnAppUpdateInfo = _OnAppUpdateInfo;
            return this;
        }

        public IAsyncOperation HandleNewVersionIfExisted()
        {
            if (_downloadOperation != null) return _downloadOperation;
            _remoteConfig =_remoteConfigRepository.Get();
            if (!_remoteConfig.Unlocked) return new SharedAsyncOperation().Fail("!_remoteConfig.Unlocked");
            var versionCode = AndroidUtils.GetVersionCode();
            HMSGameServiceManager.Instance.CheckAppUpdate(true, _remoteConfig.MinVersionCode > versionCode);
            _downloadOperation = new SharedAsyncOperation();
            return _downloadOperation;
        }

        private void _OnAppUpdateInfo(HMSGameServiceManager.OnAppUpdateInfoRes res)
        {
            _DebugOnAppUpdateInfoRes(res);
        }

        [Conditional("LOG_INFO")]
        private void _DebugOnAppUpdateInfoRes(HMSGameServiceManager.OnAppUpdateInfoRes res)
        {
            Dictionary<string, object> logDict = new()
            {
                {"Status", res.Status},
                {"RtnCode", res.RtnCode},
                {"RtnMessage", res.RtnMessage},
                {"IsExit", res.IsExit},
                {"ButtonStatus", res.ButtonStatus}
            };
            SharedLogger.Log($"{TAG}->_DebugOnAppUpdateInfoRes: {JsonConvert.SerializeObject(logDict)}");
        }

        public void ConfirmUpdateByUser()
        {
            
        }
    }
}
#endif