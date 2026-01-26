using System.Collections.Generic;

namespace Shared.SharedReport.Version
{
    [System.Serializable]
    public class VersionEntity
    {
        public string Name { get; }
        public Dictionary<string, string> Versions { get; } = new();

        public VersionEntity(string name)
        {
            Name = name;
        }

        public void AddVersion(string name, string version)
        {
            Versions.Add(name, version);
        }

        public void AddAndroidNativeVersion(string version) => AddVersion("android_native", version);
        public void AddAndroidAdapterVersion(string version) => AddVersion("android_adapter", version);
        public void AddUnityPluginVersion(string version) => AddVersion("unity_plugin", version);
    }
}