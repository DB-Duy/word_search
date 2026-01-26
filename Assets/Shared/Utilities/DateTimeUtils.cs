using System;

namespace Shared.Utilities
{
    public static class DateTimeUtils
    {
        public static string Fix2Digits(long v)
        {
            if (v < 10) return string.Format("0{0}", v); else return v.ToString();
        }

        public static string FormatHHMMSS(DateTime _start, DateTime _end)
        {
            long interval = (long)(_end - _start).TotalSeconds;

            long hour = interval / (60 * 60);
            long remain = interval % (60 * 60);
            long minutes = remain / 60;
            long seconds = remain % 60;
            return string.Format("{0}:{1}:{2}", Fix2Digits(hour), Fix2Digits(minutes), Fix2Digits(seconds));
        }

        public static string FormatMMSS(DateTime _start, DateTime _end)
        {
            long interval = (long)(_end - _start).TotalSeconds;

            long minutes = interval / 60;
            long seconds = interval % 60;
            return string.Format("{0}:{1}", Fix2Digits(minutes), Fix2Digits(seconds));
        }

        public static TimeSpan CalculateRemain(DateTime startTime, int lifeTimeInMinutes)
        {
            DateTime endTime = startTime.AddMinutes(lifeTimeInMinutes);
            return endTime - DateTime.Now;
        }

        public static string FormatRemain(DateTime startTime, int lifeTimeInMinutes)
        {
            TimeSpan timeSpan = CalculateRemain(startTime, lifeTimeInMinutes);
            return timeSpan.ToString(@"hh\:mm\:ss");
        }

        // https://briancaos.wordpress.com/2022/02/24/c-datetime-to-unix-timestamps/
        // Convert datetime to UNIX time
        public static string ToUnixTime(this DateTime dateTime)
        {
            DateTimeOffset dto = new DateTimeOffset(dateTime.ToUniversalTime());
            return dto.ToUnixTimeSeconds().ToString();
        }

        // https://briancaos.wordpress.com/2022/02/24/c-datetime-to-unix-timestamps/
        // Convert datetime to UNIX time including miliseconds
        public static string ToUnixTimeMilliSeconds(this DateTime dateTime)
        {
            DateTimeOffset dto = new DateTimeOffset(dateTime.ToUniversalTime());
            return dto.ToUnixTimeMilliseconds().ToString();
        }
    
        public static long ToUnixMilliSeconds(this DateTime dateTime)
        {
            var dto = new DateTimeOffset(dateTime.ToUniversalTime());
            return dto.ToUnixTimeMilliseconds();
        }

        /// <summary>
        /// $"{dateTime.Year}{dateTime.Month}{dateTime.Day}";
        /// </summary>
        public static string ToStringOf4Y2M2D(this DateTime dateTime) => $"{dateTime.Year}{dateTime.Month}{dateTime.Day}";
    }
}
