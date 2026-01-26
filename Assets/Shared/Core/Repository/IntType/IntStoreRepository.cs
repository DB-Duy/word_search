namespace Shared.Core.Repository.IntType
{
    public class IntStoreRepository : IntPlayerPrefsRepository
    {
        public IntStoreRepository(string name = null, int defaultValue = 0) : base(name, defaultValue)
        {
        }
    }
}