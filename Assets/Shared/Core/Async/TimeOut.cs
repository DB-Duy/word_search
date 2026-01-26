using System;

namespace Shared.Core.Async
{
    public class TimeOut
    {
        private readonly float _timeoutInSeconds;
        private DateTime _start;

        public TimeOut(float timeoutInSeconds = 8f)
        {
            _timeoutInSeconds = timeoutInSeconds;
        }

        public TimeOut Start()
        {
            _start = DateTime.Now;
            return this;
        }

        public bool IsTimeout()
        {
            var interval = (DateTime.Now - _start).TotalSeconds; 
            return interval >= _timeoutInSeconds || interval < 0;
        }
    }
}