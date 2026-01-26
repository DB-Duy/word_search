namespace Shared.Service.Tracking.Common
{
    [System.Serializable]
    public class GameResourceAction : ValueObject
    {
        private GameResourceAction(string v) : base(v)
        {
        }

        public static GameResourceAction Earned = new("earned");
        public static GameResourceAction Spent = new("spent");
    }
}