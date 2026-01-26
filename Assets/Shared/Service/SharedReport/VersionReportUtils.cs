using System.Collections.Generic;
using UnityEngine;

namespace Shared.SharedReport
{
    public static class VersionReportUtils
    {
        public static VersionVerifyRequest NewInstance(Dictionary<string, Dictionary<string, string>> versions)
        {
            var packageName = AndroidUtils.GetPackageName();
            var versionName = Application.version;
            var versionCode = AndroidUtils.GetVersionCode();

            return VersionVerifyRequest.NewInstance(packageName, versionName, $"{versionCode}", versions);
        }
    }
}