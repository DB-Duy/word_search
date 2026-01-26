using System;

namespace Shared.Service.Session
{
    public static class SessionEvents
    {
        public static Action<long> OnNewSessionCreatedEvent = delegate {  };
    }
}