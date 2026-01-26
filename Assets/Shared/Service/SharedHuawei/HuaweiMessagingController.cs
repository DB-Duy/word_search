#if HUAWEI
using System;
using System.Collections.Generic;
using System.Diagnostics;
using HmsPlugin;
using HuaweiMobileServices.Base;
using HuaweiMobileServices.Id;
using HuaweiMobileServices.Push;
using HuaweiMobileServices.Utils;
using Newtonsoft.Json;
using Shared.Common;
using Shared.SharedMessaging;
using Shared.Utils;
using UnityEngine;

namespace Shared.SharedHuawei
{
    public class HuaweiMessagingController : IHuaweiMessagingController
    {
        private const string TAG = "HuaweiMessagingController";

        public bool IsInitialized { get; private set; } = false;
        public string Token { get; private set; }
        private readonly HashSet<IMessagingTokenReceivedHandler> _handlers = new();
        private IAsyncOperation _initOperation;

        public IAsyncOperation Initialize()
        {
            if (_initOperation != null) return _initOperation;
            IsInitialized = true;
            SharedLogger.Log($"{TAG}->Initialize");
            _initOperation = new SharedAsyncOperation();
            if (!Application.isEditor)
            {
                try
                {
                    HMSPushKitManager.Instance.OnTokenSuccess = _OnTokenSuccess;
                    HMSPushKitManager.Instance.OnTokenFailure = _OnTokenFailure;

                    HMSPushKitManager.Instance.OnTokenBundleSuccess = _OnTokenBundleSuccess;
                    HMSPushKitManager.Instance.OnTokenBundleFailure = _OnTokenBundleFailure;

                    HMSPushKitManager.Instance.OnMessageSentSuccess = _OnMessageSentSuccess;
                    HMSPushKitManager.Instance.OnMessageDeliveredSuccess = _OnMessageDeliveredSuccess;
                    HMSPushKitManager.Instance.OnMessageReceivedSuccess = _OnMessageReceivedSuccess;

                    HMSPushKitManager.Instance.OnSendFailure = _OnSendFailure;
                    HMSPushKitManager.Instance.OnNotificationMessage = _OnNotificationMessage;
                    HMSPushKitManager.Instance.NotificationMessageOnStart = _NotificationMessageOnStart;

                    HMSPushKitManager.Instance.Init();
                    _GetAAID();
                }
                catch (Exception e)
                {
                    SharedLogger.LogError($"{TAG}->Initialize: {e.Message}");
                    return _initOperation.Success();
                }
            }
            SharedLogger.Log($"{TAG}->Initialize: DONE");
            return _initOperation.Success();
        }
        
        public IMessagingController AddTokenReceivedHandlers(params IMessagingTokenReceivedHandler[] handlers)
        {
            _handlers.AddRange(handlers);
            return this;
        }
        
        /// ------------------------------------------------------------------------------------------------------------
        /// Token 
        /// ------------------------------------------------------------------------------------------------------------
        private void _OnTokenSuccess(string token)
        {
            SharedLogger.Log($"{TAG}->_OnTokenSuccess: {token}");
            foreach (var handler in _handlers) handler.Handle(token);
        }

        private void _OnTokenFailure(Exception e)
        {
            SharedLogger.LogError($"{TAG}->_OnTokenFailure: {e.Message}");
        }
        
        /// ------------------------------------------------------------------------------------------------------------
        /// Token Bundle
        /// ------------------------------------------------------------------------------------------------------------
        private void _OnTokenBundleSuccess(string token, Bundle bundle)
        {
            SharedLogger.Log($"{TAG}->_OnTokenBundleSuccess: {token}");
            foreach (var handler in _handlers) handler.Handle(token);
        }

        private void _OnTokenBundleFailure(Exception exception, Bundle bundle)
        {
            SharedLogger.LogError($"{TAG}->_OnTokenBundleFailure: {exception.Message}");
        }
        /// ------------------------------------------------------------------------------------------------------------
        /// Others
        /// ------------------------------------------------------------------------------------------------------------
        private void _OnMessageSentSuccess(string msgId) => SharedLogger.Log($"{TAG}->_OnMessageSentSuccess: {msgId}");
        private void _OnMessageDeliveredSuccess(string msgId, Exception exception) => SharedLogger.LogError($"{TAG}->_OnMessageDeliveredSuccess: msgId={msgId}, exception={exception.Message}");
        private void _OnMessageReceivedSuccess(RemoteMessage remoteMessage) => _DebugRemoteMessage(remoteMessage);
        
        private void _OnSendFailure(string msgId, Exception exception) => SharedLogger.LogError($"{TAG}->_OnSendFailure: msgId={msgId}, exception={exception.Message}");
        private void _OnNotificationMessage(NotificationData entity) => _DebugNotificationData("_OnNotificationMessage", entity);
        private void _NotificationMessageOnStart(NotificationData entity) => _DebugNotificationData("_NotificationMessageOnStart", entity);

        /// ------------------------------------------------------------------------------------------------------------
        /// Utils
        /// ------------------------------------------------------------------------------------------------------------
        [Conditional("LOG_INFO")]
        private void _DebugRemoteMessage(RemoteMessage remoteMessage)
        {
            var logDict = new Dictionary<string, object>()
            {
                {"CollapseKey", remoteMessage.CollapseKey},
                {"Entity", remoteMessage.Entity},
                {"MessageId", remoteMessage.MessageId},
                {"MessageType", remoteMessage.MessageType},
                {"Notification", _Convert(remoteMessage.GetNotification)},//
                {"OriginalUrgency", remoteMessage.OriginalUrgency},
                {"Urgency", remoteMessage.Urgency},
                {"Ttl", remoteMessage.Ttl},
                {"SentTime", remoteMessage.SentTime},
                {"To", remoteMessage.To},
                {"From", remoteMessage.From},
                {"Token", remoteMessage.Token},
                {"AnalyticInfo", remoteMessage.AnalyticInfo},
                {"AnalyticInfoMap", remoteMessage.AnalyticInfoMap},
                {"AnalyticInfoMap", remoteMessage.AnalyticInfoMap},
            };
            SharedLogger.Log($"{TAG}->_DebugRemoteMessage: {JsonConvert.SerializeObject(logDict)}");
        }

        private Dictionary<string, object> _Convert(RemoteMessage.Notification notification)
        {
            return new Dictionary<string, object>()
            {
                {"Title", notification.Title},
                {"TitleLocalizationKey", notification.TitleLocalizationKey},
                {"TitleLocalizationArgs", notification.TitleLocalizationArgs},
                {"Body", notification.Body},
                {"BodyLocalizationKey", notification.BodyLocalizationKey},
                {"BodyLocalizationArgs", notification.BodyLocalizationArgs},
                {"Icon", notification.Icon},
                {"ImageUrl", notification.ImageUrl},
                {"Sound", notification.Sound},
                {"Tag", notification.Tag},
                {"Color", notification.Color},
                {"ClickAction", notification.ClickAction},
                {"IntentUri", notification.IntentUri},
                {"ChannelId", notification.ChannelId},
                {"Link", notification.Link},
                {"NotifyId", notification.NotifyId},
                {"DefaultLight", notification.DefaultLight},
                {"DefaultSound", notification.DefaultSound},
                {"DefaultVibrate", notification.DefaultVibrate},
                {"When", notification.When},
                {"LocalOnly", notification.LocalOnly},
                {"BadgeNumber", notification.BadgeNumber},
                {"AutoCancel", notification.AutoCancel},
                {"Importance", notification.Importance},
                {"Ticker", notification.Ticker},
                {"Visibility", notification.Visibility},
            };
        }

        private void _DebugNotificationData(string fromFunction, NotificationData notificationData)
        {
            var logDict = new Dictionary<string, object>()
            {
                {"MsgId", notificationData.MsgId},
                {"CmdType", notificationData.CmdType},
                {"NotifyId", notificationData.NotifyId},
                {"KeyValueJSON", notificationData.KeyValueJSON},
            };
            SharedLogger.Log($"{TAG}->{fromFunction}: {JsonConvert.SerializeObject(logDict)}");
        }
        
        private void _GetAAID()
        {
            SharedLogger.Log($"{TAG} -> GetAAID");
            HmsInstanceId inst = HmsInstanceId.GetInstance();
            ITask<AAIDResult> idResult = inst.AAID;
            idResult.AddOnSuccessListener((result) =>
            {
                SharedLogger.Log($"{TAG} -> GetAAID: result.Id:{result.Id}");
                //AAIDResult AAIDResult = result;
                //Debug.Log("AppMessaging: " + result.Id);
                //AAIDResultAction?.Invoke(result);
            }).AddOnFailureListener((exception) =>
            {

            });
        }
    }
}
#endif