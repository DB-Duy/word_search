using Newtonsoft.Json;

namespace Shared.SharedSession
{
    [System.Serializable]
    public class JavaClassImpl : IJavaClass
    {
        public string FullPath { get; }
        public string PackageName { get; }
        public string ClassName { get; }

        public JavaClassImpl(string fullPath)
        {
            if (string.IsNullOrEmpty(fullPath))
            {
                FullPath = "null";
                ClassName = "null";
                PackageName = "null";
            }
            else
            {
                FullPath = fullPath;
                var i = fullPath.LastIndexOf(".");
                ClassName = fullPath.Substring(i + 1);
                PackageName = fullPath[..i];    
            }
        }

        public override string ToString() => JsonConvert.SerializeObject(this);
    }
}