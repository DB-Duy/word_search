namespace Shared.Core.Repository.LongType
{
    public class LongStoreRepository : LongPlayerPrefsRepository
    {
        public LongStoreRepository(string name = null, long defaultValue = 0) : base(name, defaultValue)
        {
        }
    }
}