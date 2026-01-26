#if GOOGLE_PLAY && USER_RATING
using System.Collections.Generic;
using Google.Play.Common;
using Google.Play.Review;

namespace Shared.Service.StoreRating.Internal
{
    public static class Extensions
    {
        public static Dictionary<string, object> ToDict(this PlayAsyncOperation<PlayReviewInfo, ReviewErrorCode> o)
        {
            return new Dictionary<string, object>()
            {
                { "Error", o.Error },
                { "IsSuccessful", o.IsSuccessful },
            };
        }

        public static Dictionary<string, object> ToDict(this PlayAsyncOperation<VoidResult, ReviewErrorCode> o)
        {
            return new Dictionary<string, object>()
            {
                { "Error", o.Error },
                { "IsSuccessful", o.IsSuccessful }
            };
        }
    }
}
#endif