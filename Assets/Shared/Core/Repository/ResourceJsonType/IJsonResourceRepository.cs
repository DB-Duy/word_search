namespace Shared.Core.Repository.ResourceJsonType
{
    public interface IJsonResourceRepository<T>
    {
        bool IsLoaded { get; }
        void Load();
        T Get();
    }
}