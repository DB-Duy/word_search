#if HUAWEI
using System.Diagnostics;
using HmsPlugin;
using HuaweiMobileServices.RemoteConfig;
using HuaweiMobileServices.Utils;
using Shared.Common;
using Shared.Utils;
using UnityEngine;

namespace Shared.SharedHuawei
{
    public class HuaweiRemoteConfigController : IHuaweiRemoteConfigController
    {
        private const string TAG = "HuaweiRemoteConfigController";

        public bool IsInitialized { get; private set; } = false;
        private IAsyncOperation _initOperation;

        public IAsyncOperation Initialize()
        {
            if (_initOperation != null) return _initOperation;
            _initOperation = new SharedAsyncOperation();
            if (Application.isEditor) return _initOperation.Success();
            
            HMSRemoteConfigManager.Instance.OnFetchSuccess += _OnFetchSuccess;
            HMSRemoteConfigManager.Instance.OnFetchFailure += _OnFetchFailure;
            HMSRemoteConfigManager.Instance.Fetch(0);
            return _initOperation;
        }

        private void _OnFetchSuccess(ConfigValues config)
        {
            SharedLogger.Log($"{TAG}->_OnFetchSuccess:");
            HMSRemoteConfigManager.Instance.GdprApply(config);
            IsInitialized = true;
            _initOperation.Success();
            _initOperation = null;
            _Debug();
        }

        private void _OnFetchFailure(HMSException exception)
        {
            SharedLogger.Log($"{TAG}->_OnFetchFailure: {exception.Message}");
            IsInitialized = false;
            _initOperation.Fail(exception.Message);
            _initOperation = null;
        }

        [Conditional("LOG_INFO")]
        private void _Debug()
        {
            var configs = HMSRemoteConfigManager.Instance.GetMergedAll();
            foreach (var cf in configs) SharedLogger.Log($"{TAG}->_Debug: {cf.Key}={cf.Value}");
        }
    }
}
#endif