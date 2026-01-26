using System;
using Shared.Utils;
using UnityEngine;

namespace Shared.Service.Session.Android
{
    public class AndroidApplicationLifeCycleListener : AndroidJavaProxy, IAndroidApplicationLifeCycleListener
    {
        public const string MY_APPLICATION = "com.unity3d.player.MyApplication";
        public const string MY_APPLICATION_LIFE_CYCLE = "com.unity3d.player.IAndroidApplicationLifeCycleListener";
        
        private const string Tag = "AndroidApplicationLifeCycleListener";
        
        public AndroidApplicationLifeCycleListener() : base(MY_APPLICATION_LIFE_CYCLE)
        {
#if !UNITY_EDITOR
            try
            {
                using (var pluginClass = new AndroidJavaClass(MY_APPLICATION))
                {
                    var bridgeInstance = pluginClass.CallStatic<AndroidJavaObject>("getInstance");
                    bridgeInstance.Call("setAndroidApplicationLifeCycleListener", this);
                }
            }
            catch (Exception e)
            {
                Debug.LogError("setUnityInitializationListener method doesn't exist, error: " + e.Message);
            }
#endif
        }

        void onCreate()
        {
            SharedLogger.Log($"{Tag}->onCreate");
            AndroidApplicationEvents.OnCreateEvent.Invoke();
        }

        void onActivityCreated(string activityName)
        {
            SharedLogger.Log($"{Tag}->onActivityCreated: {activityName}");
            AndroidApplicationEvents.OnActivityCreatedEvent.Invoke(activityName);
        }

        void onActivityStarted(string activityName)
        {
            SharedLogger.Log($"{Tag}->onActivityStarted: {activityName}");
            AndroidApplicationEvents.OnActivityStartedEvent.Invoke(activityName);
        }

        void onActivityResumed(string activityName)
        {
            SharedLogger.Log($"{Tag}->onActivityResumed: {activityName}");
            AndroidApplicationEvents.OnActivityResumedEvent.Invoke(activityName);
        }

        void onActivityPaused(string activityName)
        {
            SharedLogger.Log($"{Tag}->onActivityPaused: {activityName}");
            AndroidApplicationEvents.OnActivityPausedEvent.Invoke(activityName);
        }

        void onActivityPreStopped(string activityName)
        {
            SharedLogger.Log($"{Tag}->onActivityPreStopped: {activityName}");
        }

        void onActivityStopped(string activityName)
        {
            SharedLogger.Log($"{Tag}->onActivityStopped: {activityName}");
            AndroidApplicationEvents.OnActivityStoppedEvent.Invoke(activityName);
        }

        void onActivitySaveInstanceState(string activityName)
        {
            SharedLogger.Log($"{Tag}->onActivitySaveInstanceState: {activityName}");
            AndroidApplicationEvents.OnActivitySaveInstanceStateEvent.Invoke(activityName);
        }

        void onActivityDestroyed(string activityName)
        {
            SharedLogger.Log($"{Tag}->onActivityDestroyed: {activityName}");
            AndroidApplicationEvents.OnActivityDestroyedEvent.Invoke(activityName);
        }
    }
}