#if UNITY_GAMING_SERVICE
using System;
using Shared.Core.Async;
using Shared.Core.IoC;
using Shared.Utils;
using Unity.Services.Core;
using UnityEngine;

namespace Shared.Service.UnityGamingService
{
    [Service]
    public class UnityGamingService : IUnityGamingService
    {
        private const string Tag = "UnityGamingService";
        
        public bool IsInitialized { get; private set; }
        private IAsyncOperation _initOperation;
    
        public IAsyncOperation Initialize()
        {
            if (_initOperation != null) return _initOperation;
            _initOperation = new SharedAsyncOperation();
            _InitAsync();
            return _initOperation;
        }
    
        private async void _InitAsync()
        {
            try
            {
                SharedLogger.Log($"{Tag}->InitAsync");
                await UnityServices.InitializeAsync();
                IsInitialized = true;
                _initOperation.Success();
            }
            catch (Exception e)
            {
                Debug.LogErrorFormat("{0}->InitAsync:: EXCEPTION: {1}", Tag, e.Message);
                Debug.LogException(e);
                // Something went wrong when checking the GeoIP, check the e.Reason and handle appropriately.
                IsInitialized = false;
                _initOperation.Fail(e.Message);
            }
        }
    }
}
#endif