namespace Shared.Service.Tracking.Common
{
    [System.Serializable]
    public class BrightSDKAction : ValueObject
    {
        private BrightSDKAction(string v) : base(v)
        {
        }
        
        public static readonly BrightSDKAction OptIn = new("opt_in");
        public static readonly BrightSDKAction OptOut = new("opt_out");
    }
}