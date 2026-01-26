namespace Shared.Service.Tracking.Common
{
    public class LogSeverity : ValueObject
    {
        private LogSeverity(string v) : base(v)
        {
        }

        public static readonly LogSeverity Exception = new("exception");
        public static readonly LogSeverity Error = new("error");
    }
}