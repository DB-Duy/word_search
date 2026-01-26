namespace Shared.Core.Repository.StringType
{
    public class StringStoreRepository : StringPlayerPrefsRepository
    {
        public StringStoreRepository(string name = null, string defaultValue = null) : base(name, defaultValue)
        {
        }
    }
}