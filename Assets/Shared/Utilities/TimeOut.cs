using System;

namespace Shared.Utils
{
    public class TimeOut
    {
        private readonly float _timeOut;
        private DateTime _startTime;

        public TimeOut(float timeOut)
        {
            _timeOut = timeOut;
        }

        public void StartTimeOut() => _startTime = DateTime.Now;
        public bool IsTimeOut => (DateTime.Now - _startTime).TotalSeconds >= _timeOut;
    }
}