using Newtonsoft.Json;

namespace Shared.Entity.Session
{
    [System.Serializable]
    public class NativeClassEntity
    {
        [JsonProperty("class_name")] public string ClassName { get; set; }
        [JsonProperty("package_name")] public string PackageName { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="NativeClassEntity"/> class.
        /// </summary>
        /// <param name="fullPath">The fully qualified class name (e.g., com.unity.NativeApplication).</param>
        public NativeClassEntity(string fullPath)
        {
            if (string.IsNullOrWhiteSpace(fullPath))
            {
                ClassName = "null";
                PackageName = "null";
            }
            else
            {
                var lastDotIndex = fullPath.LastIndexOf('.');
                if (lastDotIndex == -1)
                {
                    // No '.' found, assume it's just the class name
                    ClassName = fullPath;
                    PackageName = string.Empty;
                }
                else
                {
                    ClassName = fullPath.Substring(lastDotIndex + 1);
                    PackageName = fullPath[..lastDotIndex];
                }
            }
        }

        public NativeClassEntity()
        {
        }
    }
}