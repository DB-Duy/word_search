
namespace Shared.Core.Repository.JsonType
{
    public class JsonStoreRepository<T> : JsonPlayerPrefsRepository<T>
    {
        public JsonStoreRepository(string name = null, T defaultValue = default) : base(name, defaultValue)
        {
        }
    }
}