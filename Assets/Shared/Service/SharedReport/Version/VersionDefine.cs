using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

namespace Shared.SharedReport.Version
{
    [System.Serializable]
    public class VersionDefine
    {
        [System.Serializable]
        public class SdkVersion
        {
            [JsonProperty("id")] public string Id { get; private set; }
            [JsonProperty("name")] public string Name { get; private set; }
            [JsonProperty("unity_plugin")] public string UnityPlugin { get; private set; }
            [JsonProperty("ios_adapter")] public string IosAdapter { get; private set; }
            [JsonProperty("ios_native")] public string IosNative { get; private set; }
            [JsonProperty("android_adapter")] public string AndroidAdapter { get; private set; }
            [JsonProperty("android_native")] public string AndroidNative { get; private set; }
            [JsonProperty("notes")] public string Notes { get; private set; }
            [JsonProperty("android_native_access_point")] public string AndroidNativeAccessPoint { get; private set; }
            [JsonProperty("android_adapter_access_point")] public string AndroidAdapterAccessPoint { get; private set; }
        }

        public Dictionary<string, SdkVersion> Versions { get; private set; }

        public static VersionDefine NewInstance()
        {
            var asset = Resources.Load<TextAsset>("versions");
            var jsonString = asset.text;
            var instance = new VersionDefine
            {
                Versions = JsonConvert.DeserializeObject<Dictionary<string, SdkVersion>>(jsonString)
            };
            return instance;
        }
    }
}