using System;
using System.Collections.Generic;
using Shared.Service.AudioAds.Handlers;
using Shared.Utils;

namespace Shared
{
    public static class SharedCoreEvents
    {
        public static class AudioAds
        {
            public static readonly Action OnAdPlaybackStartedEvent = delegate { };
            public static readonly Action OnAdPlaybackCompletedEvent = delegate { };

            private static readonly HashSet<IAudioAdPlaybackStartedHandler> AdPlaybackStartedHandlers = new();
            private static readonly HashSet<IAudioAdPlaybackCompletedHandler> AdPlaybackCompletedHandlers = new();

            public static void Add(params IAudioAdPlaybackStartedHandler[] handlers) => AdPlaybackStartedHandlers.AddRange(handlers);
            public static void Add(params IAudioAdPlaybackCompletedHandler[] handlers) => AdPlaybackCompletedHandlers.AddRange(handlers);

            public static void InvokeAdPlaybackStartedHandlers()
            {
                OnAdPlaybackStartedEvent.Invoke();
                foreach (var handler in AdPlaybackStartedHandlers) handler.Handle();
            }

            public static void InvokeAdPlaybackCompletedHandlers()
            {
                OnAdPlaybackCompletedEvent.Invoke();
                foreach (var handler in AdPlaybackCompletedHandlers) handler.Handle();
            }
        }
        
        public static class Session
        {
            public static Action<long> OnNewSessionCreatedEvent = delegate {  };
            
            public static void InvokeNewSessionHandlers(long sessionId)
            {
                OnNewSessionCreatedEvent.Invoke(sessionId);
            }
        }
        
    }
}