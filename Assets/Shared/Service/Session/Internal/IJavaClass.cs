namespace Shared.SharedSession
{
    public interface IJavaClass
    {
        string FullPath { get; }
        string PackageName { get; }
        string ClassName { get; }
    }
}