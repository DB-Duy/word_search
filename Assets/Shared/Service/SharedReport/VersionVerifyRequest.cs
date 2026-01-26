using System.Collections.Generic;
using Newtonsoft.Json;

namespace Shared.SharedReport
{
    [System.Serializable]
    public class VersionVerifyRequest
    {
        [JsonProperty("versionName")] public string VersionName { get; private set; }
        [JsonProperty("versionCode")] public string VersionCode { get; private set; }
        [JsonProperty("packageName")] public string PackageName { get; private set; }

        [JsonProperty("versions")] public Dictionary<string, Dictionary<string, string>> Versions { get; private set; }

        public static VersionVerifyRequest NewInstance(string packageName, string versionName, string versionCode, Dictionary<string, Dictionary<string, string>> versions)
        {
            return new VersionVerifyRequest()
            {
                PackageName = packageName,
                VersionName = versionName,
                VersionCode = versionCode,
                Versions = versions
            };
        }
    }
}