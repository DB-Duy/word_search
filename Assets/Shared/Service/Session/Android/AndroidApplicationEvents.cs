using System;

namespace Shared.Service.Session.Android
{
    public static class AndroidApplicationEvents
    {
        public static Action OnCreateEvent = delegate { };
        public static Action<string> OnActivityCreatedEvent = delegate { };
        public static Action<string> OnActivityStartedEvent = delegate { };
        public static Action<string> OnActivityResumedEvent = delegate { };
        public static Action<string> OnActivityPausedEvent = delegate { };
        public static Action<string> OnActivityStoppedEvent = delegate { };
        public static Action<string> OnActivitySaveInstanceStateEvent = delegate { };
        public static Action<string> OnActivityDestroyedEvent = delegate { };
        
        // public static Action<string> OnSharedApplicationPauseEvent = delegate { };
        // public static Action<string> OnSharedApplicationResumeEvent = delegate { };
    }
}