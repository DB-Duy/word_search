namespace Shared.Utils
{
    public static class TypeUtils
    {
        public static string ResoleEntityResourceFullname<T>()
        {
            var filePath = typeof(T).FullName;
            filePath = filePath.Replace("Entity.", string.Empty);
            filePath = filePath.Replace(".", "/");
            return $"{filePath}";
        }

        public static string ResolvePrefabFilePath<T>()
        {
            return typeof(T).FullName
                .Replace("View.", string.Empty) //
                .Replace("Entity.", string.Empty) //
                .Replace(".", "/");
        }
    }
}