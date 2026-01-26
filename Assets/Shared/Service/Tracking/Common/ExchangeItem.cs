namespace Shared.Service.Tracking.Common
{
    public class ExchangeItem : ValueObject
    {
        public ExchangeItem(string v) : base(v) {}

        public static readonly ExchangeItem NoItem = new("null");
    }
}