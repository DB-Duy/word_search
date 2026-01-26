namespace Shared.Service.Tracking.Common
{
    [System.Serializable]
    public class GameRestartLocation : ValueObject
    {
        public GameRestartLocation(string v) : base(v)
        {
        }
    }
}