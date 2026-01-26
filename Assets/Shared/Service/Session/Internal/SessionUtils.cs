using System;

namespace Shared.Service.Session.Internal
{
    public static class SessionUtils
    {
        public static long NewSessionId() => new DateTimeOffset(DateTime.Now.ToUniversalTime()).ToUnixTimeMilliseconds();
    }
}