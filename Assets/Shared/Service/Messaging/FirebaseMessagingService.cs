#if FIREBASE
using System;
using System.Collections;
using System.Threading.Tasks;
using Shared.Core.Async;
using Shared.Core.Handler;
using Shared.Core.IoC;
using Shared.Service.Firebase;
using Shared.Service.Messaging.Handler;
using Shared.Service.SharedCoroutine;
using Shared.Utils;
using UnityEngine;

namespace Shared.Service.Messaging
{
    [Service]
    public class FirebaseMessagingService : IMessagingService, ISharedUtility
    {
        private IHandler<string> _tokenReceivedHandlerChain;
        private IHandler<string> TokenReceivedHandlerChain => _tokenReceivedHandlerChain ??= SequenceHandlerChain<string>.CreateChainFromType<IMessagingTokenReceivedHandler>();
        
        public bool IsInitialized { get; private set; } = false;
        private IAsyncOperation _initOperation;
        public IAsyncOperation Initialize()
        {
            if (_initOperation != null) return _initOperation;
            this.LogInfo(SharedLogTag.FirebaseNMessaging);
            IsInitialized = true;
            _initOperation = new SharedAsyncOperation().Success();
            this.StartSharedCoroutine(_TryInitAsync());
            return _initOperation;
        }

        private IEnumerator _TryInitAsync()
        {
            while (!FirebaseFlag.IsEnabled) yield return null;
            _RegisterCallbacks();
            yield return null;
            yield return _GetTokenAsync();
        }

        private void _RegisterCallbacks()
        {
            this.LogInfo(SharedLogTag.FirebaseNMessaging);
            try
            {
                global::Firebase.Messaging.FirebaseMessaging.TokenReceived += _OnTokenReceived;
                global::Firebase.Messaging.FirebaseMessaging.MessageReceived += _OnMessageReceived;
            }
            catch (Exception e)
            {
                this.LogError(SharedLogTag.FirebaseNMessaging, nameof(e.Message), e.Message);
            }
        }

        private IEnumerator _GetTokenAsync()
        {
            Task<string> task = null;
            try
            {
                task = global::Firebase.Messaging.FirebaseMessaging.GetTokenAsync();
            }
            catch (Exception e)
            {
                if (e.Message.Contains("TOO_MANY_REGISTRATIONS") || e.Message.Contains("SERVICE_NOT_AVAILABLE"))
                {
                    this.LogWarning(SharedLogTag.FirebaseNMessaging, nameof(e.Message), e.Message);
                }
                else
                {
                    this.LogError(SharedLogTag.FirebaseNMessaging, nameof(e.Message), e.Message);
                }
                yield break;
            }
            if (task == null) yield break;
            // Wait until the task is completed
            while (!task.IsCompleted) yield return null;
            
            if (task.IsCompletedSuccessfully)
            {
                this.LogInfo(SharedLogTag.FirebaseNMessaging, nameof(task.Result), task.Result);
            }
            else
            {
                this.LogError(SharedLogTag.FirebaseNMessaging, "task.Exception?.Message", task.Exception?.Message ?? "Unknown error");
                if (task.Exception?.Message == null) yield break;
                if (task.Exception.Message.Contains("TOO_MANY_REGISTRATIONS") || task.Exception.Message.Contains("SERVICE_NOT_AVAILABLE"))
                {
                    this.LogWarning(SharedLogTag.FirebaseNMessaging, nameof(task.Exception.Message), task.Exception.Message);
                }
                else
                {
                    this.LogError(SharedLogTag.FirebaseNMessaging, nameof(task.Exception.Message), task.Exception.Message);
#if LOG_INFO
                    Debug.LogException(task.Exception);
#endif
                }
            }
        }
        
        private void _OnTokenReceived(object sender, global::Firebase.Messaging.TokenReceivedEventArgs token)
        {
            this.LogInfo(SharedLogTag.FirebaseNMessaging, nameof(token.Token), token.Token);
            TokenReceivedHandlerChain.Handle(token.Token);
        }

        private void _OnMessageReceived(object sender, global::Firebase.Messaging.MessageReceivedEventArgs e)
        {
            this.LogInfo(SharedLogTag.FirebaseNMessaging, nameof(e.Message.Data), e.Message.Data);
        }
    }
}
#endif

// BUG Cases
// Shared.Service.Firebase.MobileFirebaseService->Initialize->Failed: 	
// Loading InitializableHandlerChain->MoveNext: {"service":"FirebaseMessagingService"}	
// [Firebase] FirebaseMessagingService->Initialize: 	
// InvalidOperationException: Don't call Firebase functions before CheckDependencies has finished	
//     at Firebase.FirebaseApp.ThrowIfCheckDependenciesRunning () [0x00000] in <00000000000000000000000000000000>:0 	
//     at Firebase.FirebaseApp.GetInstance (System.String name) [0x00000] in <00000000000000000000000000000000>:0 	
//     at Firebase.FirebaseApp.get_DefaultInstance () [0x00000] in <00000000000000000000000000000000>:0 	
//     at Firebase.Messaging.FirebaseMessagingInternal+Listener..ctor () [0x00000] in <00000000000000000000000000000000>:0 	
//     at Firebase.Messaging.FirebaseMessagingInternal+Listener.Create () [0x00000] in <00000000000000000000000000000000>:0 	
//     at Firebase.Messaging.FirebaseMessagingInternal..cctor () [0x00000] in <00000000000000000000000000000000>:0 	
//     at Firebase.Messaging.FirebaseMessaging.add_TokenReceived (System.EventHandler`1[TEventArgs] value) [0x00000] in <00000000000000000000000000000000>:0 	
//     at Shared.Service.Messaging.FirebaseMessagingService.Initialize () [0x00000] in <00000000000000000000	