using System;
using Shared.Core.Async;
using UnityEngine;

namespace Shared.SharedDevToDev.Internal
{
    public class DevToDevInitAsyncOperation : IAsyncOperation, IDevToDevInitAsyncOperation
    {
        private const string Tag = "DevToDevAsyncOperation";
        // IDevToDevAsyncOperation
        public Action OnCompleteEvent { get; set; }

        private bool _isComplete;
        public bool IsComplete
        {
            get
            {
                _isComplete = IsTimeoutAndSdkInitialized || IsAllInitSuccess;
                if (_isComplete && OnCompleteEvent != null)
                {
                    OnCompleteEvent?.Invoke();
                    OnCompleteEvent = null;
                }
                return _isComplete;
            }
        }
        public bool IsSuccess => IsAllInitSuccess;
        public string FailReason { get; }

        private bool IsTimeoutAndSdkInitialized => IsTimeOut && _isSdkInitialized;
        private bool IsAllInitSuccess => _isSdkInitialized && _isRemoteConfigInitialized;
        
        // IDevToDevInitOperation
        private bool _isSdkInitialized = false;
        private bool _isRemoteConfigInitialized = false;
        private bool IsTimeOut => _isTimerEnable && (Time.realtimeSinceStartup - _cachedRealtimeSinceStartup) >= _timeOut;
        
        // IDevToDevInitOperation internal properties
        private bool _isTimerEnable = false;
        private readonly float _timeOut;
        private float _cachedRealtimeSinceStartup;

        public DevToDevInitAsyncOperation(float timeout)
        {
            _timeOut = timeout;
            _isTimerEnable = false;
        }


        public IAsyncOperation Success()
        {
            throw new NotImplementedException();
        }

        public IAsyncOperation Fail(string reason)
        {
            throw new NotImplementedException();
        }

        public IAsyncOperation Fail(string reason, params object[] p)
        {
            throw new NotImplementedException();
        }

        public void StartInit()
        {
            if (_isTimerEnable) throw new Exception($"{Tag}->StartTimer: ERROR: _isTimerEnable");
            _isTimerEnable = true;
            _cachedRealtimeSinceStartup = Time.realtimeSinceStartup;
        }

        public void OnSdkInitialized()
        {
            _isSdkInitialized = true;
        }

        public void OnRemoteConfigInitialized()
        {
            _isRemoteConfigInitialized = true;
        }
    }
}