namespace Shared.Service.Tracking.Common
{
    public class GameMode : ValueObject
    {
        public GameMode(string v) : base(v)
        {
        }

        public static readonly GameMode NullValue = new("null");
    }
}