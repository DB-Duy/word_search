namespace Shared.Service.Tracking.Common
{
    public class TrackingScreen : ValueObject
    {
        public TrackingScreen(string v) : base(v)
        {
        }

        public static readonly TrackingScreen Unknown = new("unknown");
        public static readonly TrackingScreen Loading = new("loading");
        public static readonly TrackingScreen FakeRating = new("fake_rating");
        public static readonly TrackingScreen Ump = new("ump");
        /// <summary>
        /// USed for ParentControl
        /// </summary>
        public static readonly TrackingScreen AskAge = new("ask_age");
        /// <summary>
        /// USed for ParentControl
        /// </summary>
        public static readonly TrackingScreen PrivacyPurchase = new("privacy_purchase");
        /// <summary>
        /// USed for ParentControl
        /// </summary>
        public static readonly TrackingScreen ParentPermission = new("parent_permission");
        
    }
}