#if IN_APP_UPDATE
using System.Collections.Generic;
using Google.Play.AppUpdate;
using Google.Play.Common;

namespace Shared.Service.InAppUpdate.GooglePlay
{
    public static class Extensions
    {
        public static Dictionary<string, object> ToDict(this AppUpdateRequest request)
        {
            return new Dictionary<string, object>
            {
                { "Error", request.Error },
                { "Status", request.Status },
            };
        }

        public static Dictionary<string, object> ToDict(this PlayAsyncOperation<VoidResult, AppUpdateErrorCode> result)
        {
            return new Dictionary<string, object>
            {
                { "Error", result.Error }
            };
        }

        public static Dictionary<string, object> ToDict(this AppUpdateInfo i)
        {
            if (i == null)
                return new Dictionary<string, object>
                {
                    { "i", "null" }
                };

            return new Dictionary<string, object>
            {
                { "UpdateAvailability", i.UpdateAvailability.ToString() },
                { "UpdatePriority", i.UpdatePriority },
                { "AppUpdateStatus", i.AppUpdateStatus },
                { "BytesDownloaded", i.BytesDownloaded },
                { "TotalBytesToDownload", i.TotalBytesToDownload },
                { "AvailableVersionCode", i.AvailableVersionCode },
                { "ClientVersionStalenessDays", i.ClientVersionStalenessDays },
            };
        }

        // Fatal Exception: java.lang.Exception: InvalidOperationException : GetResult called after operation failed with error: ErrorAppNotOwned
        //    at Google.Play.Common.PlayAsyncOperation`2[TResult,TError].GetResult(Google.Play.Common.PlayAsyncOperation`2[TResult,TError])
        //    at Shared.Service.InAppUpdate.GooglePlay.Extensions.ToDict(Shared.Service.InAppUpdate.GooglePlay.Extensions)
        //    at Shared.Service.InAppUpdate.GooglePlay.State.CheckForUpdateState+<Execute>d__0.MoveNext(Shared.Service.InAppUpdate.GooglePlay.State.CheckForUpdateState+<Execute>d__0)
        //    at UnityEngine.SetupCoroutine.InvokeMoveNext(UnityEngine.SetupCoroutine)
        public static Dictionary<string, object> ToDict(this PlayAsyncOperation<AppUpdateInfo, AppUpdateErrorCode> result)
        {
            var i = result.Error == AppUpdateErrorCode.NoError ? result.GetResult() : null;
            return new Dictionary<string, object>
            {
                { "Error", result.Error },
                { "IsSuccessful", result.IsSuccessful },
                { "i", i == null ? "null" : i.ToDict() }
            };
        }
    }
}
#endif