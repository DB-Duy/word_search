namespace Shared.Core.Repository.ResourceJsonType
{
    public interface IDictionaryResourceRepository<T>
    {
        bool IsLoaded { get; }
        void Load();
        T Get(string id, T defaultValue = default);
    }
}